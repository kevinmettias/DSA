using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithEvenOddRatioISolution's, the
// same methods CountSubarraysWithEvenOddRatioITests proves correct. a and b
// are fixed at 1/1 so roughly half of all subarrays qualify (x <= y),
// keeping neither arm's inner loop short-circuited into a near-empty scan.
[MemoryDiagnoser]
public class CountSubarraysWithEvenOddRatioIBenchmarks
{
    private const int Ratio = 1;
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 4011;

    [Params(200, 1_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountSubarraysWithEvenOddRatioISolution.CountByBruteForce(_nums, Ratio, Ratio);

    [Benchmark]
    public int FenwickPrefixSweep() =>
        CountSubarraysWithEvenOddRatioISolution.CountByFenwickPrefixSweep(_nums, Ratio, Ratio);
}
