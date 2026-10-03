using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximizeSubarrayGCDScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeSubarrayGCDScoreSolution's, the same
// methods MaximizeSubarrayGCDScoreSolutionTests proves correct.
//
// Sizes are per arm. Brute force tries every doubling subset of every subarray -
// genuinely exponential in subarray length - so it stops at 12
// (FindTheNumberOfSubsequencesWithEqualGcdBenchmarks' own precedent for "size the
// baseline can survive"); the bottleneck-scan arm never enumerates a subset at all,
// so its O(n^2) scan runs on to LC 3574's own bound of 1,500. The two are compared at
// the lengths both run.
public class MaximizeSubarrayGCDScoreBenchmarks
{
    private const int Seed = 3574; // LC problem number
    private const int MaxValueExclusive = 1_000;
    private const int K = 2;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [8, 12];

    public static IEnumerable<int> BottleneckScanSizes => [.. BruteForceSizes, 150, 1_500];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = BottleneckScanSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, MaxValueExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int length) =>
        MaximizeSubarrayGCDScoreSolution.MaxScoreByBruteForce(_numsByLength[length], K);

    [Benchmark]
    [ArgumentsSource(nameof(BottleneckScanSizes))]
    public long BottleneckGcdScan(int length) =>
        MaximizeSubarrayGCDScoreSolution.MaxScoreByBottleneckGcdScan(_numsByLength[length], K);
}
