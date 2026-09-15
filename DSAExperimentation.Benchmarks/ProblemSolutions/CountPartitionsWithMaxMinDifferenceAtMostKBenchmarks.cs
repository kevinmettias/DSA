using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountPartitionsWithMaxMinDifferenceAtMostK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountPartitionsWithMaxMinDifferenceAtMostKSolution's, the same methods
// CountPartitionsWithMaxMinDifferenceAtMostKTests proves correct.
[MemoryDiagnoser]
public class CountPartitionsWithMaxMinDifferenceAtMostKBenchmarks
{
    private const int NumsSeed = 3578;

    private int[] _nums = [];

    [Params(500, 4_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = MaxMinPartitionWorkloads.BuildNums(Length, NumsSeed);

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        CountPartitionsWithMaxMinDifferenceAtMostKSolution.CountPartitionsByBruteForce(
            _nums, MaxMinPartitionScenario.MaxMinDifference);

    [Benchmark]
    public int SlidingWindowDeque() =>
        CountPartitionsWithMaxMinDifferenceAtMostKSolution.CountPartitionsBySlidingWindowDeque(
            _nums, MaxMinPartitionScenario.MaxMinDifference);
}
