using DSAExperimentation.LeetCode.MinimumCostToMakeAtLeastOneValidPathInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToMakeAtLeastOneValidPathInAGrid;

// Harness only. Both the linear-scan Dijkstra baseline and the heap-backed frontier
// are MinimumCostToMakeAtLeastOneValidPathInAGridSolution's - this file pins them to
// LeetCode's published examples plus the degenerate single-cell and single-row grids
// that exercise the rectangular bounds rather than only square ones.
public sealed partial class MinimumCostToMakeAtLeastOneValidPathInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LC 1368's own three examples: alternating right/left rows needing one
            // override per row change, arrows that already form a valid path, and a
            // 2x2 needing exactly one override.
            { [[1, 1, 1, 1], [2, 2, 2, 2], [1, 1, 1, 1], [2, 2, 2, 2]], 3 },
            { [[1, 1, 3], [3, 2, 2], [1, 1, 4]], 0 },
            { [[1, 2], [4, 3]], 1 },

            // Start is already the target, so nothing is ever crossed.
            { [[1]], 0 },

            // Single rows: every arrow already points along the walk, or none does.
            { [[1, 1, 1]], 0 },
            { [[2, 2, 2]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByLinearScanDijkstra_LeetCodeExamples_ReturnsFewestArrowOverrides(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByLinearScanDijkstra(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByHeapDijkstra_LeetCodeExamples_ReturnsFewestArrowOverrides(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByHeapDijkstra(grid));
}
