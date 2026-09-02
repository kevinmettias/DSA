using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumSumOfSubsequenceWithNonAdjacentElementsSolution's, the same methods
// MaximumSumOfSubsequenceWithNonAdjacentElementsTests proves correct. Every
// index gets exactly one query (a full rewrite of the array, in a random
// order) - the case that most separates the two strategies, since the
// baseline rescans all of Length on every single one of those Length queries
// (O(n^2) overall) while the segment tree pays O(log n) per update. Confirmed
// locally at Length=1_000 (~1.4x faster) and Length=20_000 (~35x faster), the
// gap widening with scale as O(n log n) vs. O(n^2) predicts.
[MemoryDiagnoser]
public class MaximumSumOfSubsequenceWithNonAdjacentElementsBenchmarks
{
    private const int MaxAbsoluteValue = 1_000;
    private const int Seed = 3165;

    [Params(1_000, 20_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-MaxAbsoluteValue, MaxAbsoluteValue)).ToArray();

        var order = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
        _queries = order.Select(position => new[] { position, random.Next(-MaxAbsoluteValue, MaxAbsoluteValue) }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecomputeDP() =>
        MaximumSumOfSubsequenceWithNonAdjacentElementsSolution.MaximumSumByRecomputeDP(_nums, _queries);

    [Benchmark]
    public int SegmentTreeMerge() =>
        MaximumSumOfSubsequenceWithNonAdjacentElementsSolution.MaximumSumBySegmentTreeMerge(_nums, _queries);
}
