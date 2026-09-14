using DSAExperimentation.LeetCode.MaximumNumberOfPointsFromGridQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfPointsFromGridQueries;

// Harness only. Both strategies are MaximumNumberOfPointsFromGridQueriesSolution's -
// the per-query re-simulation and the single shared min-heap flood fill - so this
// file only pins them to LeetCode's published examples and the hand-traced grids the
// original suite carried, including the two cases where the start cell itself blocks
// every query and the one where a below-query cell is unreachable because the region
// around it is not.
public sealed class MaximumNumberOfPointsFromGridQueriesTests
{
    public static TheoryData<int[][], int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1: at query 5 the bottom-right 1 is walled off by two
            // 5s, so it is below the threshold and still uncounted; query 6 lets the
            // 5s through and picks it up, and query 2 admits the start cell alone.
            { [[1, 2, 3], [2, 5, 7], [3, 5, 1]], [5, 6, 2], [5, 8, 1] },
            // LeetCode example 2: the start cell's own value blocks the only query.
            { [[5, 2, 1], [1, 1, 2]], [3], [0] },
            // 1 3 / 2 4 - query 2 only ever admits the start cell (1); query 5
            // admits every cell, since the whole 2x2 grid is < 5.
            { [[1, 3], [2, 4]], [2, 5], [1, 4] },
            // Single cell: strictly-greater-than is required, so a query equal to
            // the only value gets 0 points, one greater gets the single point.
            { [[1]], [1, 2], [0, 1] },
            // 3 1 2 (one row): start value 3 blocks both query<=3 cases outright;
            // query 10 floods the whole row.
            { [[3, 1, 2]], [2, 3, 10], [0, 0, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByFloodFillPerQuery_LeetCodeExamples_ReturnsPerQueryReachableCellCounts(
        int[][] grid, int[] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByFloodFillPerQuery(grid, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByMinHeapFrontier_LeetCodeExamples_ReturnsPerQueryReachableCellCounts(
        int[][] grid, int[] queries, int[] expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByMinHeapFrontier(grid, queries));
}
