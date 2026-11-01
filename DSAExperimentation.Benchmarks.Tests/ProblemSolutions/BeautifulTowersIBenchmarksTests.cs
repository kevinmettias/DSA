using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulTowersIBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this asserts the total is positive: Setup draws every max height from 1 upward, so a sum of zero
// could only mean an arm placed no tower.
public sealed partial class BeautifulTowersIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 1_000)] tower counts.
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_TwoHundredTowerSkyline_ReturnsAPositiveTotal() =>
        Assert.True(BuildHarness().BruteForce() > 0);

    private static BeautifulTowersIBenchmarks BuildHarness()
    {
        var harness = new BeautifulTowersIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
