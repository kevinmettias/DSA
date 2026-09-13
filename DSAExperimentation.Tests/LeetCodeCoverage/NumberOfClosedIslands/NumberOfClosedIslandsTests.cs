using DSAExperimentation.LeetCode.NumberOfClosedIslands;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfClosedIslands;

// Harness only. Both flood-fill strategies are NumberOfClosedIslandsSolution's -
// this file just pins them to LeetCode's three published examples, the
// border-touching component the count has to exclude, and an all-water grid
// where every cell reaches the edge.
public sealed class NumberOfClosedIslandsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [1, 1, 1, 1, 1, 1, 1, 0],
                    [1, 0, 0, 0, 0, 1, 1, 0],
                    [1, 0, 1, 0, 1, 1, 1, 0],
                    [1, 0, 0, 0, 0, 1, 0, 1],
                    [1, 1, 1, 1, 1, 1, 1, 0],
                ],
                2
            },
            {
                [
                    [0, 0, 1, 0, 0],
                    [0, 1, 0, 1, 0],
                    [0, 1, 1, 1, 0],
                ],
                1
            },
            {
                [
                    [1, 1, 1, 1, 1, 1, 1],
                    [1, 0, 0, 0, 0, 0, 1],
                    [1, 0, 1, 1, 1, 0, 1],
                    [1, 0, 1, 0, 1, 0, 1],
                    [1, 0, 1, 1, 1, 0, 1],
                    [1, 0, 0, 0, 0, 0, 1],
                    [1, 1, 1, 1, 1, 1, 1],
                ],
                2
            },
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
    [MemberData(nameof(Examples))]
    public void CountClosedIslandsByNaiveFloodFill_LeetCodeExamples_CountsOnlyInteriorComponents(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfClosedIslandsSolution.CountClosedIslandsByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountClosedIslandsByDepthFirstSearch_LeetCodeExamples_CountsOnlyInteriorComponents(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfClosedIslandsSolution.CountClosedIslandsByDepthFirstSearch(grid));
}
