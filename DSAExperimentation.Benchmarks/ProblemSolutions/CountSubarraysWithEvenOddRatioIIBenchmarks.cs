using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithEvenOddRatioIISolution's, the
// same methods CountSubarraysWithEvenOddRatioIITests proves correct. Length
// is deliberately kept well under LC 4013's own 1e5 ceiling - BruteForce is
// O(n^2) and exists only as a correctness baseline, so a size that already
// makes the quadratic cost visible is enough; the point of this benchmark is
// the crossover, not reproducing the contest's own worst case.
[MemoryDiagnoser]
public class CountSubarraysWithEvenOddRatioIIBenchmarks
{
    private const int Ratio = 1;
    private const int MaxValueExclusive = 1_000_000_000;
    private const int Seed = 4013;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountSubarraysWithEvenOddRatioIISolution.CountByBruteForce(_nums, Ratio, Ratio);

    [Benchmark]
    public long FenwickPrefixSweep() =>
        CountSubarraysWithEvenOddRatioIISolution.CountByFenwickPrefixSweep(_nums, Ratio, Ratio);
}
