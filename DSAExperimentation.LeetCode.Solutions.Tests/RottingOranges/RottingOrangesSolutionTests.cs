using DSAExperimentation.LeetCode.Solutions.RottingOranges;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RottingOranges;

// LeetCode 994. Rotting Oranges. See RottingOrangesSolution for the two strategies:
// a naive independent BFS per fresh orange, and this repo's own multi-source BFS
// composed over Queue<TElement>. Harness only - the cases are stated once and
// each strategy gets its own theory so a failure names the arm that broke. The
// cases are the smallest grids: one cell of each kind, a fresh orange beside a
// rotten one, and a single row the rot has to cross end to end. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class RottingOrangesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0]], 0 },
            { [[1]], -1 },
            { [[2]], 0 },
            { [[1, 2]], 1 },
            { [[2, 1, 1, 1, 1]], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void OrangesRottingByPerCellBfs_Cases_ReturnsMinutesUntilAllRot(
        int[][] grid, int expected) =>
        Assert.Equal(expected, RottingOrangesSolution.OrangesRottingByPerCellBfs(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void OrangesRottingByMultiSourceBfs_Cases_ReturnsMinutesUntilAllRot(
        int[][] grid, int expected) =>
        Assert.Equal(expected, RottingOrangesSolution.OrangesRottingByMultiSourceBfs(grid));
}
