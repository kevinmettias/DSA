using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeSubarrayGCDScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeSubarrayGCDScoreSolution's, the same
// methods MaximizeSubarrayGCDScoreTests proves correct. Brute force tries
// every doubling subset of every subarray - genuinely exponential in subarray
// length - so Length stays small enough for that arm to finish in reasonable
// time (FindTheNumberOfSubsequencesWithEqualGcdBenchmarks' own precedent for
// "size the baseline can survive"); the bottleneck-scan arm's whole point is
// that it never enumerates a subset at all.
[MemoryDiagnoser]
public class MaximizeSubarrayGCDScoreBenchmarks
{
    private const int Seed = 3574; // LC problem number
    private const int MaxValueExclusive = 1_000;
    private const int K = 2;

    private int[] _nums = [];

    [Params(8, 12)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximizeSubarrayGCDScoreSolution.MaxScoreByBruteForce(_nums, K);

    [Benchmark]
    public long BottleneckGcdScan() => MaximizeSubarrayGCDScoreSolution.MaxScoreByBottleneckGcdScan(_nums, K);
}
