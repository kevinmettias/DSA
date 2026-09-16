using DSAExperimentation.LeetCode.MinimumObstacleRemovalToReachCorner;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumObstacleRemovalToReachCorner;

// Harness only: both strategies are MinimumObstacleRemovalToReachCornerSolution's
// - this file just pins them to LeetCode's published examples, plus the cases a
// 0/1-weighted shortest path gets wrong most easily: a detour that costs nothing
// at all, a corridor with no detour available, and the single-cell grid where the
// source already is the corner.
public sealed partial class MinimumObstacleRemovalToReachCornerTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 1, 1], [1, 1, 0], [1, 1, 0]], 2 },
            { [[0, 1, 0, 0, 0], [0, 1, 0, 1, 0], [0, 0, 0, 1, 0]], 0 },
            { [[0, 0, 0], [0, 1, 0], [0, 0, 0]], 0 },
            { [[0, 1, 1, 1, 0]], 3 },
            { [[0, 1], [1, 0]], 1 },
            { [[0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumObstaclesByArrayScanDijkstra_LeetCodeExamples_ReturnsFewestObstaclesOnTheWayToTheCorner(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected,
            MinimumObstacleRemovalToReachCornerSolution.MinimumObstaclesByArrayScanDijkstra(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumObstaclesByWeightedGridDijkstra_LeetCodeExamples_ReturnsFewestObstaclesOnTheWayToTheCorner(
        int[][] grid, int expected) =>
        Assert.Equal(
            expected,
            MinimumObstacleRemovalToReachCornerSolution.MinimumObstaclesByWeightedGridDijkstra(grid));
}
