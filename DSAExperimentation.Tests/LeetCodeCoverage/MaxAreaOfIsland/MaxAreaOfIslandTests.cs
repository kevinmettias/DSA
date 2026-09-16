using DSAExperimentation.LeetCode.MaxAreaOfIsland;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxAreaOfIsland;

// Harness only. Both strategies are MaxAreaOfIslandSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class MaxAreaOfIslandTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0],
                    [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0],
                    [0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0],
                    [0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0],
                    [0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0],
                    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0],
                    [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0],
                    [0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0],
                ],
                6
            },
            { [[0, 0], [0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByNaiveFloodFill_LeetCodeExamples_ReturnsLargestIslandArea(int[][] grid, int expected) =>
        Assert.Equal(expected, MaxAreaOfIslandSolution.MaxAreaByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByDepthFirstSearch_LeetCodeExamples_ReturnsLargestIslandArea(int[][] grid, int expected) =>
        Assert.Equal(expected, MaxAreaOfIslandSolution.MaxAreaByDepthFirstSearch(grid));
}
