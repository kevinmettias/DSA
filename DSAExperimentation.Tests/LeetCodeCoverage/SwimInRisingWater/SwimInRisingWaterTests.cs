using DSAExperimentation.LeetCode.SwimInRisingWater;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwimInRisingWater;

// Harness only. Both search strategies are SwimInRisingWaterSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class SwimInRisingWaterTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [0, 2],
                    [1, 3],
                ],
                3
            },
            {
                [
                    [0, 1, 2, 3, 4],
                    [24, 23, 22, 21, 5],
                    [12, 13, 14, 15, 16],
                    [11, 17, 18, 19, 20],
                    [10, 9, 8, 7, 6],
                ],
                16
            },
            {
                [[0]],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByBinarySearchFloodFill_LeetCodeExamples_ReturnsMinimumPossibleTime(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SwimInRisingWaterSolution.MinTimeByBinarySearchFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByHeapDijkstra_LeetCodeExamples_ReturnsMinimumPossibleTime(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SwimInRisingWaterSolution.MinTimeByHeapDijkstra(grid));
}
