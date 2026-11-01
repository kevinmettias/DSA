using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindPolygonWithTheLargestPerimeterBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, and arms that all answer -1 would agree too, so
// this holds the baseline to what Setup's workload guarantees. Sixteen sides each at least 1 make
// the total comfortably larger than twice the longest one on every run, so a polygon exists and the
// answer is a perimeter rather than -1.
public sealed partial class FindPolygonWithTheLargestPerimeterBenchmarksTests
{
    // The smaller of the side counts both arms run, 16 and 20, the one whose subset arm is affordable.
    private const int SmallestSideCount = 16;

    [Fact]
    public void BruteForceSubsets_SeededSides_FindsAPolygon() =>
        Assert.True(BuildHarness().BruteForceSubsets(SmallestSideCount) > 0);

    private static FindPolygonWithTheLargestPerimeterBenchmarks BuildHarness()
    {
        var harness = new FindPolygonWithTheLargestPerimeterBenchmarks();
        harness.Setup();

        return harness;
    }
}
