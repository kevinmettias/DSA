using DSAExperimentation.LeetCode.Solutions.CountSubIslands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSubIslands;

// Harness only. Both flood fills are CountSubIslandsSolution's - this file pins
// them to the three shapes the pre-migration
// test carried: every island covered, one island straddling grid1 water, and a
// grid2 with no land at all.
public sealed partial class CountSubIslandsSolutionTests
{
    public static TheoryData<int[][], int[][], int> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void CountByRecursiveFloodFill_Cases_CountsGrid2IslandsFullyCoveredByGrid1(
        int[][] grid1, int[][] grid2, int expected)
    {
        var actual = CountSubIslandsSolution.CountByRecursiveFloodFill(grid1, grid2);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByDepthFirstSearchTraverse_Cases_CountsGrid2IslandsFullyCoveredByGrid1(
        int[][] grid1, int[][] grid2, int expected)
    {
        var actual = CountSubIslandsSolution.CountByDepthFirstSearchTraverse(grid1, grid2);
        Assert.Equal(expected, actual);
    }
}
