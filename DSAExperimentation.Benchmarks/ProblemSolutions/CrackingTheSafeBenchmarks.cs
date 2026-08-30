using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cracking the Safe (LC 753): a hand-specialized recursive greedy-with-undo search
// (StringBuilder + HashSet<string>, mirroring the classic accepted solution) vs.
// this repo's generic Backtrack.TrySearch engine closed over the same
// StringBuilder-answer/Set-of-seen-passwords shape the test uses. K is fixed at 2
// (binary passwords, the problem's own running example) and N is [Params]-scaled -
// the search space grows as 2^N, so N stays small (2, 3) the same way
// SudokuSolverBenchmarks/NQueensBenchmarks keep their fixed-size inputs small
// enough for a reasonable iteration budget while still isolating the constant-
// factor cost of Backtrack.TrySearch's generic delegate dispatch from an
// equivalent purpose-built recursion.
[MemoryDiagnoser]
public class CrackingTheSafeBenchmarks
{
    private const int K = 2;

    [Params(2, 3)]
    public int N;

    [Benchmark(Baseline = true)]
    public string SpecializedRecursive() => CrackSafeSpecialized(N, K);

    [Benchmark]
    public string BacktrackEngine() => CrackSafeWithBacktrackEngine(N, K);

    private static string CrackSafeSpecialized(int n, int k)
    {
        var total = (int)Math.Pow(k, n);
        var answer = new StringBuilder(new string('0', n));
        var visited = new HashSet<string> { new string('0', n) };

        bool Search()
        {
            if (visited.Count == total)
            {
                return true;
            }

            var prefix = answer.ToString(answer.Length - (n - 1), n - 1);

            for (var digit = 0; digit < k; digit++)
            {
                var password = prefix + (char)('0' + digit);
                if (visited.Add(password))
                {
                    answer.Append((char)('0' + digit));

                    if (Search())
                    {
                        return true;
                    }

                    answer.Length -= 1;
                    visited.Remove(password);
                }
            }

            return false;
        }

        Search();
        return answer.ToString();
    }

    private static string CrackSafeWithBacktrackEngine(int n, int k)
    {
        var state = new State(n, k);
        string? answer = null;

        Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Visited.Count == s.Total,
            Candidates: s => CandidateDigits(s),
            Choose: (s, digit) =>
            {
                s.Visited.TryAdd(NextPassword(s, digit));
                s.Answer.Append((char)('0' + digit));
            },
            Unchoose: (s, digit) =>
            {
                s.Visited.TryRemove(CurrentSuffix(s));
                s.Answer.Length -= 1;
            },
            OnSolution: s =>
            {
                answer = s.Answer.ToString();
                return true;
            }));

        return answer!;
    }

    private static IEnumerable<int> CandidateDigits(State state)
    {
        for (var digit = 0; digit < state.K; digit++)
        {
            if (!state.Visited.Has(NextPassword(state, digit)))
            {
                yield return digit;
            }
        }
    }

    private static string NextPassword(State state, int digit) => Suffix(state, state.N - 1) + (char)('0' + digit);

    private static string CurrentSuffix(State state) => Suffix(state, state.N);

    private static string Suffix(State state, int length) => state.Answer.ToString(state.Answer.Length - length, length);

    private sealed class State
    {
        public State(int n, int k)
        {
            N = n;
            K = k;
            Total = (int)Math.Pow(k, n);
            Answer = new StringBuilder(new string('0', n));
            Visited = new Set<string>();
            Visited.TryAdd(new string('0', n));
        }

        public int N { get; }
        public int K { get; }
        public int Total { get; }
        public StringBuilder Answer { get; }
        public Set<string> Visited { get; }
    }
}
