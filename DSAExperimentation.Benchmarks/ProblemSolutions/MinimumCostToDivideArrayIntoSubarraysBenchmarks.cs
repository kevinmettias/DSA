using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToDivideArrayIntoSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToDivideArrayIntoSubarraysSolution's,
// the same methods MinimumCostToDivideArrayIntoSubarraysTests proves correct.
// Both strategies are O(n^2) states x O(n) transition either way - the DP itself,
// not the search space, is quadratic - so this is measuring cache overhead
// (hand-rolled Dictionary vs. this repo's Memoizer), not a complexity gap.
[MemoryDiagnoser]
public class MinimumCostToDivideArrayIntoSubarraysBenchmarks
{
    private const int Seed = 3500; // LC problem number
    private const int K = 5;

    [Params(50, 200)]
    public int Length;

    private int[] _nums = null!;
    private int[] _cost = null!;

    [GlobalSetup]
    public void Setup() => (_nums, _cost) = MinimumCostToDivideArrayIntoSubarraysWorkloads.Build(Length, Seed);

    [Benchmark(Baseline = true)]
    public long DictionaryMemo() =>
        MinimumCostToDivideArrayIntoSubarraysSolution.MinimumCostByDictionaryMemo(_nums, _cost, K);

    [Benchmark]
    public long MemoizedPartition() =>
        MinimumCostToDivideArrayIntoSubarraysSolution.MinimumCostByMemoizedPartition(_nums, _cost, K);
}
