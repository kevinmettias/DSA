using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfSubarraysWithANDValueOfK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfSubarraysWithANDValueOfKSolution's, the
// same methods NumberOfSubarraysWithANDValueOfKTests proves correct. Neither
// arm ever exits early on a match, so K's value doesn't bias the comparison -
// it's fixed only so the workload is deterministic.
[MemoryDiagnoser]
public class NumberOfSubarraysWithANDValueOfKBenchmarks
{
    private const int Seed = 3209;
    private const int K = 0;
    private const int MaxValueExclusive = 1_000_000_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => NumberOfSubarraysWithANDValueOfKSolution.CountByBruteForce(_nums, K);

    [Benchmark]
    public long AndValueCompression() => NumberOfSubarraysWithANDValueOfKSolution.CountByAndValueCompression(_nums, K);
}
