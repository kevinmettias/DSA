using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Permutation Sequence (LC 60): this repo's own Backtrack.TrySearch enumerating
// permutations in lexicographic order and stopping once it reaches the k-th
// (O(n!) worst case - every permutation before it gets materialized) vs. the
// factorial-number-system (factoradic) selection over DynamicArray<int>
// (O(n^2), never materializes a permutation it isn't returning). K is fixed at
// n! - the lexicographically last permutation - so BruteForceEnumeration is
// always forced through its full worst case instead of an early exit on a small
// k making it look artificially competitive.
[MemoryDiagnoser]
public class PermutationSequenceBenchmarks
{
    [Params(6, 8)]
    public int N;

    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var factorial = 1;
        for (var i = 1; i <= N; i++)
        {
            factorial *= i;
        }

        _k = factorial;
    }

    [Benchmark(Baseline = true)]
    public string BacktrackEnumeration()
    {
        var n = N;
        var target = _k;
        var state = new State(n);
        var found = "";
        var count = 0;

        Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Values.Count == n,
            Candidates: s => s.Values.Count == n ? [] : Enumerable.Range(1, n).Where(d => !s.Used[d - 1]),
            Choose: (s, d) => { s.Used[d - 1] = true; s.Values.Add(d); },
            Unchoose: (s, d) => { s.Used[d - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            OnSolution: s =>
            {
                count++;
                if (count != target)
                {
                    return false;
                }

                found = string.Concat(s.Values);
                return true;
            }));

        return found;
    }

    [Benchmark]
    public string FactoradicSelection()
    {
        var n = N;
        var k = _k - 1;
        var digits = new DynamicArray<int>();
        for (var d = 1; d <= n; d++)
        {
            digits.Add(d);
        }

        var factorial = new int[n + 1];
        factorial[0] = 1;
        for (var i = 1; i <= n; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        var result = new StringBuilder();
        for (var remaining = n; remaining >= 1; remaining--)
        {
            var index = k / factorial[remaining - 1];
            k %= factorial[remaining - 1];
            result.Append(digits.Get(index));
            digits.RemoveAt(index);
        }

        return result.ToString();
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }
}
