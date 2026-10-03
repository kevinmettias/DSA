using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarrayXORWithBoundedRangeSolution's, the
// same methods MaximumSubarrayXORWithBoundedRangeSolutionTests proves correct. Values
// are drawn below LC 3845's own 2^15 ceiling, and High is set well below that
// ceiling so a meaningful fraction of positions break a run, rather than the whole
// array degenerating into a single valid run.
public class MaximumSubarrayXORWithBoundedRangeBenchmarks
{
    private const int RandomSeed = 3845;
    private const int MaxValueExclusive = 1 << 15;
    private const int Low = 0;
    private const int High = 30_000;

    private int[] _nums = [];

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 0, MaxValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBruteForce(_nums, Low, High);

    [Benchmark]
    public int BitTrieSegments() =>
        MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBitTrieSegments(_nums, Low, High);
}
