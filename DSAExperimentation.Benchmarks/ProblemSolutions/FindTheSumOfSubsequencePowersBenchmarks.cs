using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheSumOfSubsequencePowers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheSumOfSubsequencePowersSolution's, the same
// methods FindTheSumOfSubsequencePowersSolutionTests proves correct. k is fixed well
// below Length.
//
// Sizes are per arm. The 2^n brute force stops at 16 elements; the O(n^4 k)
// threshold-counting DP runs on to LC 3098's own bound of 50, and the two are compared
// at the lengths both run.
public class FindTheSumOfSubsequencePowersBenchmarks
{
    private const int MaxValueExclusive = 1_000_000;
    private const int Seed = 3098;
    private const int K = 4;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [10, 16];

    public static IEnumerable<int> ThresholdCountingSizes => [.. BruteForceSizes, 32, 50];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = ThresholdCountingSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, MaxValueExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForce(int length) =>
        FindTheSumOfSubsequencePowersSolution.SumOfPowersByBruteForce(_numsByLength[length], K);

    [Benchmark]
    [ArgumentsSource(nameof(ThresholdCountingSizes))]
    public int ThresholdCounting(int length) =>
        FindTheSumOfSubsequencePowersSolution.SumOfPowersByThresholdCounting(_numsByLength[length], K);
}
