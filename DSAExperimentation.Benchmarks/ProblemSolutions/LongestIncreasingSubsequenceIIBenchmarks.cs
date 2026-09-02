using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Increasing Subsequence II (LC 2407): the textbook O(n^2) DP (for each i,
// rescan every earlier j checking both the increasing and the <= k value-gap
// constraints) vs. this repo's own SegmentTree<int,MaxOperation<int>> keyed directly
// by value (not by rank - the [num-k, num-1] window lives in value-space, so
// coordinate compression as NumberOfLongestIncreasingSubsequenceBenchmarks uses would
// break the window) - O(log maxValue) range-max query plus point-update per element
// instead of an O(n) inner scan, so O(n log maxValue) overall.
[MemoryDiagnoser]
public class LongestIncreasingSubsequenceIIBenchmarks
{
    private const int RandomSeed = 2407; // LC problem number
    private const int K = 5;

    [Params(2_000, 6_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming()
    {
        var dp = new int[_values.Length];
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (_values[j] < _values[i] && _values[i] - _values[j] <= K && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }

            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    [Benchmark]
    public int SegmentTreeValueWindow()
    {
        var maxValue = _values.Max();
        var tree = new SegmentTree<int, MaxOperation<int>>(new int[maxValue + 1]);
        var best = 0;

        foreach (var num in _values)
        {
            var lo = Math.Max(0, num - K);
            var predecessor = tree.Query(lo, num - 1);
            var length = predecessor + 1;

            tree.Update(num, Math.Max(tree.Query(num, num), length));
            best = Math.Max(best, length);
        }

        return best;
    }
}
