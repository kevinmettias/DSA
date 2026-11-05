using DSAExperimentation.LeetCode.Solutions.MaxAreaOfIsland;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaxAreaOfIsland;

// Harness only. Both strategies are MaxAreaOfIslandSolution's - this file just
// pins them to a square grid of water alone, which holds no island and so answers
// 0. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaxAreaOfIslandSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0], [0, 0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxAreaByNaiveFloodFill_Cases_ReturnsLargestIslandArea(int[][] grid, int expected) =>
        Assert.Equal(expected, MaxAreaOfIslandSolution.MaxAreaByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxAreaByDepthFirstSearch_Cases_ReturnsLargestIslandArea(int[][] grid, int expected) =>
        Assert.Equal(expected, MaxAreaOfIslandSolution.MaxAreaByDepthFirstSearch(grid));
}
