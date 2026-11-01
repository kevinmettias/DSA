using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulTowersIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this asserts the total is positive, since every drawn max height starts at 1. Same algorithm as
// Beautiful Towers I at II's much larger [Params] sizes, so the two classes are covered by parallel harnesses.
public sealed partial class BeautifulTowersIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 8_000)] tower counts.
    private const int SmallestLength = 1_000;

    [Fact]
    public void BruteForce_ThousandTowerSkyline_ReturnsAPositiveTotal() =>
        Assert.True(BuildHarness().BruteForce() > 0);

    private static BeautifulTowersIIBenchmarks BuildHarness()
    {
        var harness = new BeautifulTowersIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
