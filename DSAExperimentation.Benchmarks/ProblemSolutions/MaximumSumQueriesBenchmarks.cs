using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumQueriesSolution's, the same methods
// MaximumSumQueriesTests proves correct. BruteForceScan answers each query
// independently by rescanning every index - O(n*q) - while SweepWithSegmentTree
// MergeSorts indices and queries by descending nums1/x, admits indices into a
// SegmentTree<long, MaxOperation<long>> keyed by nums2's coordinate-compressed rank as
// their nums1 threshold is met, and answers each query with one range-max query over
// BinarySearch.LowerBound(y)..lastRank - O((n + q) log n).
//
// Both arms take LeetCode's own arrays, which are already the prepared input, so there
// is nothing for [GlobalSetup] to hoist beyond generating them (#17.4).
[MemoryDiagnoser]
public class MaximumSumQueriesBenchmarks
{
    private const int RandomSeed = 2736; // LC problem number
    private const int ValueRange = 1_000_000;

    [Params(200, 2_000)]
    public int Length;

    [Params(200, 2_000)]
    public int QueryCount;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueRange)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueRange)).ToArray();
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(1, ValueRange), random.Next(1, ValueRange) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() =>
        MaximumSumQueriesSolution.MaxSumsByBruteForceScan(_nums1, _nums2, _queries);

    [Benchmark]
    public int[] SweepWithSegmentTree() =>
        MaximumSumQueriesSolution.MaxSumsBySweepWithSegmentTree(_nums1, _nums2, _queries);
}
