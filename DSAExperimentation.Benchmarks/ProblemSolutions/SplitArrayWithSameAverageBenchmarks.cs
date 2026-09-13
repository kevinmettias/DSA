using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SplitArrayWithSameAverage;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitArrayWithSameAverageSolution's, the same
// methods SplitArrayWithSameAverageTests proves correct - the textbook O(2^n)
// all-subsets brute force against the O(n * (n/2) * sum) subset-sum-with-a-
// required-count DP over this repo's own Memoizer. The two Length values
// deliberately straddle the crossover: at 16, brute force still edges out the
// DP's per-call Dictionary/closure overhead; at 24, 2^24 subsets makes brute
// force ~350x slower than the DP in a local dry run, the exponential-vs-
// polynomial gap this problem's real (n up to 30) constraints exist to force.
[MemoryDiagnoser]
public class SplitArrayWithSameAverageBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 805;

    [Params(16, 24)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() =>
        _nums = SplitArrayWithSameAverageWorkloads.BuildValues(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public bool BruteForceSubsets() =>
        SplitArrayWithSameAverageSolution.CanSplitBySubsetMasks(_nums);

    [Benchmark]
    public bool MemoizedSubsetSumWithCount() =>
        SplitArrayWithSameAverageSolution.CanSplitByMemoizedSubsetSum(_nums);
}
