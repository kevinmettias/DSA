using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StatisticsFromALargeSample;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StatisticsFromALargeSampleSolution's, the same
// methods StatisticsFromALargeSampleTests proves correct. AverageCountPerValue
// scales the total sample size while the bucket range stays fixed at [0, 255] -
// the shape LeetCode itself fixes - so SampleExpansion's O(total) allocation grows
// while CumulativeBinarySearch's stays O(256) regardless.
[MemoryDiagnoser]
public class StatisticsFromALargeSampleBenchmarks
{
    private const int ValueRange = 256;

    // Random counts are drawn from [1, AverageCountPerValue * MaxCountMultiplier],
    // so the sampled mean lands near AverageCountPerValue.
    private const int MaxCountMultiplier = 2;

    // Fixed seed so the bucket draw is identical from run to run.
    private const int CountSeed = 1;

    [Params(100, 5_000)]
    public int AverageCountPerValue;

    private long[] _count = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(CountSeed);
        _count = new long[ValueRange];

        for (var i = 0; i < ValueRange; i++)
        {
            _count[i] = random.Next(1, (AverageCountPerValue * MaxCountMultiplier) + 1);
        }
    }

    [Benchmark(Baseline = true)]
    public double[] ExpandAndIndex() =>
        StatisticsFromALargeSampleSolution.ComputeStatisticsBySampleExpansion(_count);

    [Benchmark]
    public double[] CumulativeSumBinarySearch() =>
        StatisticsFromALargeSampleSolution.ComputeStatisticsByCumulativeBinarySearch(_count);
}
