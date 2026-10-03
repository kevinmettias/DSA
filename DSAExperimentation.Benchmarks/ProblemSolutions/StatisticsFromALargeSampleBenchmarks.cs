using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.StatisticsFromALargeSample;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StatisticsFromALargeSampleSolution's, the same
// methods StatisticsFromALargeSampleSolutionTests proves correct. AverageCountPerValue
// scales the total sample size while the bucket range stays fixed at [0, 255] -
// the shape LeetCode itself fixes - so SampleExpansion's O(total) allocation grows
// while CumulativeBinarySearch's stays O(256) regardless. The counts come from
// StatisticsFromALargeSampleWorkloads, which keeps the sample's mode unique as LC 1093
// promises.
public class StatisticsFromALargeSampleBenchmarks
{
    // Fixed seed so the bucket draw is identical from run to run.
    private const int CountSeed = 1;

    private long[] _count = [];

    [Params(100, 5_000)]
    public int AverageCountPerValue { get; set; }

    [GlobalSetup]
    public void Setup() => _count = StatisticsFromALargeSampleWorkloads.BuildCounts(AverageCountPerValue, CountSeed);

    [Benchmark(Baseline = true)]
    public double[] ExpandAndIndex() =>
        StatisticsFromALargeSampleSolution.ComputeStatisticsBySampleExpansion(_count);

    [Benchmark]
    public double[] CumulativeSumBinarySearch() =>
        StatisticsFromALargeSampleSolution.ComputeStatisticsByCumulativeBinarySearch(_count);
}
