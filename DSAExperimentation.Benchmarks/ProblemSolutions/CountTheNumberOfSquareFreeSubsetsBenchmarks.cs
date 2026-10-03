using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountTheNumberOfSquareFreeSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfSquareFreeSubsetsSolution's, the same
// methods CountTheNumberOfSquareFreeSubsetsSolutionTests proves correct (TwoSumBenchmarks
// precedent). Brute force is 2^n subsets, so Length stays small enough for that arm to
// finish in reasonable time - the bitmask-DP arm's whole point is that it doesn't care
// how large n gets, only how many of [1, 30]'s 18 square-free values appear.
public class CountTheNumberOfSquareFreeSubsetsBenchmarks
{
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 31;
    private const int Seed = 2572;

    private int[] _nums = [];

    [Params(15, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, MinValueInclusive, MaxValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountTheNumberOfSquareFreeSubsetsSolution.CountByBruteForce(_nums);

    [Benchmark]
    public long BitmaskMemo() => CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo(_nums);
}
