using DSAExperimentation.LeetCode.Solutions.MinimumCostToMakeAtLeastOneValidPathInAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToMakeAtLeastOneValidPathInAGrid;

// Harness only. Both the linear-scan Dijkstra baseline and the heap-backed frontier
// are MinimumCostToMakeAtLeastOneValidPathInAGridSolution's - this file pins them to
// the degenerate single-cell and single-row grids
// that exercise the rectangular bounds rather than only square ones.
public sealed partial class MinimumCostToMakeAtLeastOneValidPathInAGridSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // Start is already the target, so nothing is ever crossed.
            { [[1]], 0 },

            // Single rows: every arrow already points along the walk, or none does.
            { [[1, 1, 1]], 0 },
            { [[2, 2, 2]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCostByLinearScanDijkstra_Cases_ReturnsFewestArrowOverrides(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByLinearScanDijkstra(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCostByHeapDijkstra_Cases_ReturnsFewestArrowOverrides(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByHeapDijkstra(grid));
}
