using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PerfectRectangleBenchmarks (ARCHITECTURE 17.9): the baseline's verdict against
// the one the workload's shape fixes, which the generic check cannot supply - it holds the corner
// toggle to the baseline, and two shared rejections would pass as agreement. Setup tiles the grid with
// unit squares from GridSize alone, which is a genuine perfect cover by construction.
public sealed partial class PerfectRectangleBenchmarksTests
{
    private const int SmallestGridSize = 20;

    [Fact]
    public void IsRectangleCoverByPairwiseOverlap_UnitSquareTiling_ReportsAPerfectCover() =>
        Assert.True(BuildHarness().IsRectangleCoverByPairwiseOverlap());

    private static PerfectRectangleBenchmarks BuildHarness()
    {
        var harness = new PerfectRectangleBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
