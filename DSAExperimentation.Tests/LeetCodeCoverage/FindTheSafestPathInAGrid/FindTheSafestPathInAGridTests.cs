using DSAExperimentation.LeetCode.FindTheSafestPathInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheSafestPathInAGrid;

// Harness only: the algorithms live in FindTheSafestPathInAGridSolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class FindTheSafestPathInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // (0,0) is itself a thief, so no path can ever visit a cell safer
            // than 0 - the minimum along any path is pinned to the start cell.
            { [[1, 0, 0], [0, 0, 0], [0, 0, 1]], 0 },
            // The only thief is at (0,2); the start cell's own safeness (2) caps
            // every path's bottleneck, and a path achieving exactly 2 exists.
            { [[0, 0, 1], [0, 0, 0], [0, 0, 0]], 2 },
            // Two thieves at opposite corners: the safeness-3 cells form an
            // unreachable diagonal (no 4-directional path connects them), so the
            // best achievable bottleneck is 2, not the naive upper bound of 3.
            { [[0, 0, 0, 1], [0, 0, 0, 0], [0, 0, 0, 0], [1, 0, 0, 0]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSafenessFactorByBclBfs_LeetCodeExamples_ReturnsMaxBottleneckSafeness(int[][] grid, int expected)
        => Assert.Equal(expected, FindTheSafestPathInAGridSolution.MaximumSafenessFactorByBclBfs(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSafenessFactorByHeap_LeetCodeExamples_ReturnsMaxBottleneckSafeness(int[][] grid, int expected)
        => Assert.Equal(expected, FindTheSafestPathInAGridSolution.MaximumSafenessFactorByHeap(grid));
}
