using DSAExperimentation.LeetCode.Solutions.RegionsCutBySlashes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RegionsCutBySlashes;

// Harness only: both strategies live in RegionsCutBySlashesSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke - which now includes the 3x3-expansion flood fill the
// benchmark used as its untested baseline arm. The cases are an uncut grid and a
// single cut cell. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RegionsCutBySlashesSolutionTests
{
    public static TheoryData<string[], int> Cases =>
        new()
        {
            // Nothing is cut at all, so the whole grid is one region.
            { ["  ", "  "], 1 },
            // A single cell with a single cut: the smallest grid that is not one
            // region, and the only case with no cross-cell edges to union.
            { ["/"], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRegionsByExpandedFloodFill_Cases_ReturnsRegionCount(
        string[] grid, int expected) =>
        Assert.Equal(expected, RegionsCutBySlashesSolution.CountRegionsByExpandedFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRegionsByDisjointSetTriangles_Cases_ReturnsRegionCount(
        string[] grid, int expected) =>
        Assert.Equal(expected, RegionsCutBySlashesSolution.CountRegionsByDisjointSetTriangles(grid));
}
