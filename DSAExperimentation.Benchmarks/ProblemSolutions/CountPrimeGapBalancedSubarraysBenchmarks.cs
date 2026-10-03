using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountPrimeGapBalancedSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPrimeGapBalancedSubarraysSolution's, the same
// methods CountPrimeGapBalancedSubarraysSolutionTests proves correct.
public class CountPrimeGapBalancedSubarraysBenchmarks
{
    private const int Seed = 3589;
    private const int ValueUpperBoundExclusive = 5_000;
    private const int K = 500;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, 1, ValueUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountPrimeGapBalancedSubarraysSolution.CountByBruteForce(_nums, K);

    [Benchmark]
    public long PrimeWindowDeque() => CountPrimeGapBalancedSubarraysSolution.CountByPrimeWindowDeque(_nums, K);
}
