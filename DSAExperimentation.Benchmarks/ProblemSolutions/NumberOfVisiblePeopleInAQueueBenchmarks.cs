using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfVisiblePeopleInAQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfVisiblePeopleInAQueueSolution's, the same
// methods NumberOfVisiblePeopleInAQueueTests proves correct. Heights are a random
// permutation so no person's answer short-circuits the brute-force scan early
// (DailyTemperaturesBenchmarks' own precedent for this workload shape).
[MemoryDiagnoser]
public class NumberOfVisiblePeopleInAQueueBenchmarks
{
    private const int RandomSeed = 1944; private int[] _heights = [];

    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() =>
        NumberOfVisiblePeopleInAQueueSolution.CountVisibleByBruteForceScan(_heights);

    [Benchmark]
    public int[] MonotonicStackSweep() =>
        NumberOfVisiblePeopleInAQueueSolution.CountVisibleByMonotonicStackSweep(_heights);
}
