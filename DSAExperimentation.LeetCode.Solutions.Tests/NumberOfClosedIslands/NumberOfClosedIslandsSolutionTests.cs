using DSAExperimentation.LeetCode.Solutions.NumberOfClosedIslands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfClosedIslands;

// Harness only. Both flood-fill strategies are NumberOfClosedIslandsSolution's -
// this file just pins them to the border-touching component the count has to
// exclude and an all-water grid where every cell reaches the edge. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class NumberOfClosedIslandsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            {
                // (0,0) is its own water component touching the top-left border
                // and must not be counted; the interior 2x2 block at rows/cols
                // 1-2 is the only closed island.
                [
                    [0, 1, 1, 1, 1],
                    [1, 0, 0, 1, 1],
                    [1, 0, 0, 1, 1],
                    [1, 1, 1, 1, 1],
                    [1, 1, 1, 1, 1],
                ],
                1
            },
            { [[0, 0], [0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountClosedIslandsByNaiveFloodFill_Cases_CountsOnlyInteriorComponents(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfClosedIslandsSolution.CountClosedIslandsByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountClosedIslandsByDepthFirstSearch_Cases_CountsOnlyInteriorComponents(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfClosedIslandsSolution.CountClosedIslandsByDepthFirstSearch(grid));
}
