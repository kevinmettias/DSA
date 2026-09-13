using DSAExperimentation.LeetCode.RegionsCutBySlashes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RegionsCutBySlashes;

// Harness only: both strategies live in RegionsCutBySlashesSolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke - which now includes the 3x3-expansion
// flood fill the benchmark used as its untested baseline arm.
public sealed class RegionsCutBySlashesTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { [" /", "/ "], 2 },
            { [" /", "  "], 1 },
            { ["/\\", "\\/"], 5 },
            // Nothing is cut at all, so the whole grid is one region.
            { ["  ", "  "], 1 },
            // A single cell with a single cut: the smallest grid that is not one
            // region, and the only case with no cross-cell edges to union.
            { ["/"], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRegionsByExpandedFloodFill_LeetCodeExamples_ReturnsRegionCount(
        string[] grid, int expected) =>
        Assert.Equal(expected, RegionsCutBySlashesSolution.CountRegionsByExpandedFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRegionsByDisjointSetTriangles_LeetCodeExamples_ReturnsRegionCount(
        string[] grid, int expected) =>
        Assert.Equal(expected, RegionsCutBySlashesSolution.CountRegionsByDisjointSetTriangles(grid));
}
