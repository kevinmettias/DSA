using DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVisitedCellsInAGrid;

// Harness only. Both strategies - the O(rows + cols)-per-pop brute-force scan and the
// Reduce.Graph walk over JumpGridTopology - are
// MinimumNumberOfVisitedCellsInAGridSolution's; this file just pins them to LeetCode's
// published examples, plus the 1x1 grid where the start already IS the target and a
// two-row grid whose every jump is a single step.
public sealed class MinimumNumberOfVisitedCellsInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[3, 4, 2, 1], [4, 2, 3, 1], [2, 1, 0, 0], [2, 4, 0, 0]], 4 },
            { [[3, 4, 2, 1], [4, 2, 1, 1], [2, 1, 1, 0], [3, 4, 1, 0]], 3 },
            { [[2, 1, 0], [1, 0, 0]], -1 },
            { [[0]], 1 },
            { [[1, 1, 1], [1, 1, 0]], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinVisitedCellsByBruteForceScan_LeetCodeExamples_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected, MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByBruteForceScan(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinVisitedCellsByReduceGraph_LeetCodeExamples_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected, MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByReduceGraph(grid));
}
