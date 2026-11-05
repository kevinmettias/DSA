using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfPointsFromGridQueries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfPointsFromGridQueries;

// Harness only. Both strategies are MaximumNumberOfPointsFromGridQueriesSolution's -
// the per-query re-simulation and the single shared min-heap flood fill - so this
// file only pins them to the hand-traced grids the
// original suite carried, including the two cases where the start cell itself blocks
// every query and the one where a below-query cell is unreachable because the region
// around it is not.
public sealed partial class MaximumNumberOfPointsFromGridQueriesSolutionTests
{
    public static TheoryData<int[][], int[], int[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void MaxPointsByFloodFillPerQuery_Cases_ReturnsPerQueryReachableCellCounts(
        int[][] grid, int[] queries, int[] expected)
    {
        var actual = MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByFloodFillPerQuery(grid, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxPointsByMinHeapFrontier_Cases_ReturnsPerQueryReachableCellCounts(
        int[][] grid, int[] queries, int[] expected)
    {
        var actual = MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByMinHeapFrontier(grid, queries);

        Assert.Equal(expected, actual);
    }
}
