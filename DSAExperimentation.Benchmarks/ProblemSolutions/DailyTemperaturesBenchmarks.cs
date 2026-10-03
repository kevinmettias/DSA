using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DailyTemperatures;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DailyTemperaturesSolution's, the same methods
// DailyTemperaturesSolutionTests proves correct. Temperatures are drawn uniformly from
// LC 739's 30..100 range, so values repeat the way LeetCode's do and the days near the
// top of the range send the brute-force scan far ahead for anything warmer.
public class DailyTemperaturesBenchmarks
{
    private const int RandomSeed = 739; // LC problem number

    private const int LowestTemperature = 30;

    // One past LC 739's hottest day, 100.
    private const int TemperatureUpperBoundExclusive = 101;

    private int[] _temperatures = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _temperatures = SeededDraws.Values(Length, LowestTemperature, TemperatureUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() => DailyTemperaturesSolution.WaitDaysByBruteForceScan(_temperatures);

    [Benchmark]
    public int[] MonotonicStackSweep() => DailyTemperaturesSolution.WaitDaysByMonotonicStackSweep(_temperatures);
}
