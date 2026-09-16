using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StatisticsFromALargeSampleBenchmarks (ARCHITECTURE 17.9): both arms
// answer the same question - the summary statistics of one bucket-count array - by different
// routes, so a harness whose arms disagree is timing two different problems. Setup's bucket
// counts are seeded, so the same parameter must rebuild the same workload, or two published
// numbers were never comparable in the first place.
public sealed partial class StatisticsFromALargeSampleBenchmarksTests
{
    private const int SmallestAverageCountPerValue = 100;

    // Both strategies compute the same statistics from the same integer bucket counts through
    // a different order of operations, so their double results are compared relatively rather
    // than bit-for-bit.
    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void Setup_SameAverageCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ExpandAndIndex()),
            AnswerText.Of(BuildHarness().ExpandAndIndex()));

    [Fact]
    public void ExpandAndIndex_AgreesWithCumulativeSumBinarySearch()
    {
        var harness = BuildHarness();
        var expanded = harness.ExpandAndIndex();
        var binarySearch = harness.CumulativeSumBinarySearch();

        Assert.Equal(expanded.Length, binarySearch.Length);

        for (var i = 0; i < expanded.Length; i++)
        {
            Assert.True(IsWithinRelativeTolerance(expanded[i], binarySearch[i]));
        }
    }

    [Fact]
    public void CumulativeSumBinarySearch_AgreesWithExpandAndIndex()
    {
        var harness = BuildHarness();
        var binarySearch = harness.CumulativeSumBinarySearch();
        var expanded = harness.ExpandAndIndex();

        Assert.Equal(binarySearch.Length, expanded.Length);

        for (var i = 0; i < binarySearch.Length; i++)
        {
            Assert.True(IsWithinRelativeTolerance(binarySearch[i], expanded[i]));
        }
    }

    private static StatisticsFromALargeSampleBenchmarks BuildHarness()
    {
        var harness = new StatisticsFromALargeSampleBenchmarks
        {
            AverageCountPerValue = SmallestAverageCountPerValue,
        };

        harness.Setup();

        return harness;
    }

    private static bool IsWithinRelativeTolerance(double left, double right)
    {
        var scale = Math.Max(Math.Abs(left), Math.Abs(right));

        return Math.Abs(left - right) <= RelativeTolerance * scale;
    }
}
