using DSAExperimentation.LeetCode.CountSubIslands;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubIslands;

// Harness only. Both flood fills are CountSubIslandsSolution's - this file pins
// them to LeetCode's two published examples plus the three shapes the pre-migration
// test carried: every island covered, one island straddling grid1 water, and a
// grid2 with no land at all.
public sealed class CountSubIslandsTests
{
    public static TheoryData<int[][], int[][], int> Examples =>
        new()
        {
            {
                [[1, 1, 1, 0, 0], [0, 1, 1, 1, 1], [0, 0, 0, 0, 0], [1, 0, 0, 0, 0], [1, 1, 0, 1, 1]],
                [[1, 1, 1, 0, 0], [0, 0, 1, 1, 1], [0, 1, 0, 0, 0], [1, 0, 1, 1, 0], [0, 1, 0, 1, 0]],
                3
            },
            {
                [[1, 0, 1, 0, 1], [1, 1, 1, 1, 1], [0, 0, 0, 0, 0], [1, 1, 1, 1, 1], [1, 0, 1, 0, 1]],
                [[0, 0, 0, 0, 0], [1, 1, 1, 1, 1], [0, 1, 0, 1, 0], [0, 1, 0, 1, 0], [1, 0, 0, 0, 1]],
                2
            },

            // grid2's three islands - {(0,0),(0,1),(1,1)}, {(2,0)}, {(2,2)} - each
            // land straight onto grid1 land, so all three qualify.
            {
                [[1, 1, 0], [0, 1, 1], [1, 0, 1]],
                [[1, 1, 0], [0, 1, 0], [1, 0, 1]],
                3
            },

            // The larger island {(0,0),(0,1),(0,2),(1,0),(1,1)} touches grid1[0][0] = 0
            // and is excluded; the isolated {(2,2)} still counts.
            {
                [[0, 1, 1], [1, 1, 0], [0, 0, 1]],
                [[1, 1, 1], [1, 1, 0], [0, 0, 1]],
                1
            },

            { [[1, 1], [1, 1]], [[0, 0], [0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByRecursiveFloodFill_LeetCodeExamples_CountsGrid2IslandsFullyCoveredByGrid1(
        int[][] grid1, int[][] grid2, int expected) =>
        Assert.Equal(expected, CountSubIslandsSolution.CountByRecursiveFloodFill(grid1, grid2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByDepthFirstSearchTraverse_LeetCodeExamples_CountsGrid2IslandsFullyCoveredByGrid1(
        int[][] grid1, int[][] grid2, int expected) =>
        Assert.Equal(expected, CountSubIslandsSolution.CountByDepthFirstSearchTraverse(grid1, grid2));
}
