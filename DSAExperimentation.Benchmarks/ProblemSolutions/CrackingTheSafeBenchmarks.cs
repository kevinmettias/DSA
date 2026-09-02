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
        var state = new SpecializedSearchState(
            n, k, new StringBuilder(new string('0', n)), new HashSet<string> { new string('0', n) });

        Search(state);
        return state.Answer.ToString();
    }

    private static bool Search(SpecializedSearchState state)
    {
        if (state.Visited.Count == state.Total)
        {
            return true;
        }

        var prefix = state.Answer.ToString(state.Answer.Length - (state.N - 1), state.N - 1);

        for (var digit = 0; digit < state.K; digit++)
        {
            var password = prefix + (char)('0' + digit);
            if (state.Visited.Add(password) && TryExtend(state, password, digit))
            {
                return true;
            }
        }

        return false;
    }

    // The "try password, recurse, undo on failure" branch from Search's digit loop:
    // appends the digit, recurses, and only rolls back the append/visited-add if the
    // recursive search did not find a full Eulerian path from here.
    private static bool TryExtend(SpecializedSearchState state, string password, int digit)
    {
        state.Answer.Append((char)('0' + digit));

        if (Search(state))
        {
            return true;
        }

        state.Answer.Length -= 1;
        state.Visited.Remove(password);
        return false;
    }

    private static string CrackSafeWithBacktrackEngine(int n, int k)
    {
        var state = new State(n, k);
        string? answer = null;

        var steps = new BacktrackingSteps<State, int>(
            IsSolution: IsComplete,
            Candidates: CandidateDigits,
            Choose: ChooseDigit,
            Unchoose: UnchooseDigit,
            OnSolution: s =>
            {
                answer = s.Answer.ToString();
                return true;
            });

        Backtrack.TrySearch<State, int>(state, steps);

        return answer!;
    }

    private static bool IsComplete(State state) => state.Visited.Count == state.Total;

    private static void ChooseDigit(State state, int digit)
    {
        var password = NextPassword(state, digit);
        state.Visited.TryAdd(password);
        state.Answer.Append((char)('0' + digit));
    }

    private static void UnchooseDigit(State state, int digit)
    {
        state.Visited.TryRemove(CurrentSuffix(state));
        state.Answer.Length -= 1;
    }

    private static IEnumerable<int> CandidateDigits(State state)
    {
        for (var digit = 0; digit < state.K; digit++)
        {
            var candidatePassword = NextPassword(state, digit);
            if (!state.Visited.Has(candidatePassword))
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

    private sealed record SpecializedSearchState(int N, int K, StringBuilder Answer, HashSet<string> Visited)
    {
        public int Total => (int)Math.Pow(K, N);
    }
}
