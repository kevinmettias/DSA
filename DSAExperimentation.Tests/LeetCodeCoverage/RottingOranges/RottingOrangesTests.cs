using DSAExperimentation.LeetCode.RottingOranges;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RottingOranges;

// LeetCode 994. Rotting Oranges. See RottingOrangesSolution for the two strategies:
// a naive independent BFS per fresh orange, and this repo's own multi-source BFS
// composed over Queue<TElement>. Harness only - the examples are stated once and
// each strategy gets its own theory so a failure names the arm that broke.
public sealed partial class RottingOrangesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[2, 1, 1], [1, 1, 0], [0, 1, 1]], 4 },
            { [[2, 1, 1], [0, 1, 1], [1, 0, 1]], -1 },
            { [[0, 2]], 0 },
            { [[0]], 0 },
            { [[1]], -1 },
            { [[2]], 0 },
            { [[1, 2]], 1 },
            { [[2, 1, 1, 1, 1]], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrangesRottingByPerCellBfs_LeetCodeExamples_ReturnsMinutesUntilAllRot(
        int[][] grid, int expected) =>
        Assert.Equal(expected, RottingOrangesSolution.OrangesRottingByPerCellBfs(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrangesRottingByMultiSourceBfs_LeetCodeExamples_ReturnsMinutesUntilAllRot(
        int[][] grid, int expected) =>
        Assert.Equal(expected, RottingOrangesSolution.OrangesRottingByMultiSourceBfs(grid));
}
