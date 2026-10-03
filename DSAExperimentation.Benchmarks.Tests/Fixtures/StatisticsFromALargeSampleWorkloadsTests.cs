using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for StatisticsFromALargeSampleWorkloads (ARCHITECTURE 17.7). The reading depends
// on the counts being a sample LC 1093 could pose: one count per value in [0, 255], each at least one,
// a total of at most 10^9, and - the guarantee the extra sample exists for - a unique mode. Both of
// StatisticsFromALargeSampleBenchmarks' averages are checked at its own seed, since the tie the extra
// sample breaks depends on the draw.
public sealed partial class StatisticsFromALargeSampleWorkloadsTests
{
    // Mirrors StatisticsFromALargeSampleBenchmarks' own private CountSeed and its two averages.
    private const int Seed = 1;
    private const int SmallestAverage = 100;
    private const int LargestAverage = 5_000;

    private const long MaxTotalSamples = 1_000_000_000;

    public static TheoryData<int> Averages => new([SmallestAverage, LargestAverage]);

    [Theory]
    [MemberData(nameof(Averages))]
    public void BuildCounts_BenchmarkAverages_HaveExactlyOneLargestCount(int averageCountPerValue)
    {
        var counts = StatisticsFromALargeSampleWorkloads.BuildCounts(averageCountPerValue, Seed);

        Assert.Single(counts, count => count == counts.Max());
    }

    [Theory]
    [MemberData(nameof(Averages))]
    public void BuildCounts_BenchmarkAverages_AreOnePositiveCountPerValueInsideTheTotalCap(int averageCountPerValue)
    {
        var counts = StatisticsFromALargeSampleWorkloads.BuildCounts(averageCountPerValue, Seed);

        Assert.Equal(StatisticsFromALargeSampleWorkloads.ValueRange, counts.Length);
        Assert.All(counts, count => Assert.True(count >= 1));
        Assert.InRange(counts.Sum(), 1, MaxTotalSamples);
    }

    [Fact]
    public void BuildCounts_SameSeed_ReturnsTheSameCounts() =>
        Assert.Equal(
            StatisticsFromALargeSampleWorkloads.BuildCounts(SmallestAverage, Seed),
            StatisticsFromALargeSampleWorkloads.BuildCounts(SmallestAverage, Seed));
}
