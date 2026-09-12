using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DailyTemperatures;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DailyTemperaturesSolution's, the same methods
// DailyTemperaturesTests proves correct. Temperatures are a random permutation so
// no day's answer short-circuits the brute-force scan early.
[MemoryDiagnoser]
public class DailyTemperaturesBenchmarks
{
    private const int RandomSeed = 739; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _temperatures = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _temperatures = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() => DailyTemperaturesSolution.WaitDaysByBruteForceScan(_temperatures);

    [Benchmark]
    public int[] MonotonicStackSweep() => DailyTemperaturesSolution.WaitDaysByMonotonicStackSweep(_temperatures);
}
