using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubarraysWithCostLessThanOrEqualToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithCostLessThanOrEqualToKSolution's,
// the same methods CountSubarraysWithCostLessThanOrEqualToKTests proves correct.
[MemoryDiagnoser]
public class CountSubarraysWithCostLessThanOrEqualToKBenchmarks
{
    private const int Seed = 3835;
    private const int ValueUpperBoundExclusive = 1_000;
    private const long K = 5_000;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountSubarraysWithCostLessThanOrEqualToKSolution.CountByBruteForce(_nums, K);

    [Benchmark]
    public long MonotonicDeques() => CountSubarraysWithCostLessThanOrEqualToKSolution.CountByMonotonicDeques(_nums, K);
}
