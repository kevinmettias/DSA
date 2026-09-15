using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarrayXORWithBoundedRangeSolution's, the
// same methods MaximumSubarrayXORWithBoundedRangeTests proves correct. High is set
// well below the generated value ceiling so a meaningful fraction of positions
// break a run, rather than the whole array degenerating into a single valid run.
[MemoryDiagnoser]
public class MaximumSubarrayXORWithBoundedRangeBenchmarks
{
    private const int RandomSeed = 3845;
    private const int MaxValueExclusive = 1 << 16;
    private const int Low = 0;
    private const int High = 60_000;

    private int[] _nums = [];

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBruteForce(_nums, Low, High);

    [Benchmark]
    public int BitTrieSegments() =>
        MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBitTrieSegments(_nums, Low, High);
}
