using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AmbiguousCoordinatesBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// each other on Setup's seeded parenthesized digit run, and this asserts the run yields at least one coordinate -
// an empty answer would make that agreement vacuous.
public sealed partial class AmbiguousCoordinatesBenchmarksTests
{
    // The smaller of Setup's [Params(8, 10)] digit-run lengths.
    private const int SmallestLength = 8;

    [Fact]
    public void RebuildAndRescan_EightDigitRun_OffersAtLeastOneCoordinate() =>
        Assert.NotEmpty(BuildHarness().RebuildAndRescan());

    private static AmbiguousCoordinatesBenchmarks BuildHarness()
    {
        var harness = new AmbiguousCoordinatesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
