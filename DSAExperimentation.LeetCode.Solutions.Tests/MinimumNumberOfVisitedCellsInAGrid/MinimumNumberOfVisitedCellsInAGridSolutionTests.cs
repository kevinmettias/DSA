using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfVisitedCellsInAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfVisitedCellsInAGrid;

// Harness only. Both strategies - the O(rows + cols)-per-pop brute-force scan and the
// Reduce.Graph walk over JumpGridTopology - are
// MinimumNumberOfVisitedCellsInAGridSolution's; this file just pins them to a
// two-row grid whose every jump is a single step. LeetCode's published examples run
// in CatalogCaseTests.
public sealed partial class MinimumNumberOfVisitedCellsInAGridSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[1, 1, 1], [1, 1, 0]], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinVisitedCellsByBruteForceScan_Cases_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected, MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByBruteForceScan(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinVisitedCellsByReduceGraph_Cases_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected, MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByReduceGraph(grid));
}
