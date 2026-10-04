using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.MinimumObstacleRemovalToReachCorner;

namespace DSAExperimentation.LeetCode.Tests.MinimumObstacleRemovalToReachCorner;

// Harness only: both strategies are MinimumObstacleRemovalToReachCornerSolution's
// - this file just pins them to LeetCode's published examples, plus the cases a
// 0/1-weighted shortest path gets wrong most easily: a detour that costs nothing
// at all, a corridor with no detour available, and the single-cell grid where the
// source already is the corner. The obstacle-cost graph the Dijkstra strategy is
// handed is asserted on its own, edge by edge.
public sealed partial class MinimumObstacleRemovalToReachCornerSolutionTests
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

    // LeetCode's first grid, 0 1 1 / 1 1 0 / 1 1 0. An edge costs the obstacle flag of
    // the cell it ENTERS, listed up, down, left, right: (0,0) enters (1,0) and (0,1), both
    // 1s; the centre enters three 1s and, to its right, the 0 at (1,2); the far corner
    // enters the 0 above it and the 1 to its left.
    [Fact]
    public void BuildObstacleCostGraph_LeetCodeFirstExample_WeighsEachEdgeByTheCellItEnters()
    {
        var nodes = MinimumObstacleRemovalToReachCornerSolution.BuildObstacleCostGraph(
            [[0, 1, 1], [1, 1, 0], [1, 1, 0]]);
        var fromStart = EdgesOf(nodes, (0, 0));
        var fromCentre = EdgesOf(nodes, (1, 1));
        var fromCorner = EdgesOf(nodes, (2, 2));

        Assert.Equal(9, nodes.Count);
        Assert.Equal([(1, 1, 0), (1, 0, 1)], fromStart);
        Assert.Equal([(1, 0, 1), (1, 2, 1), (1, 1, 0), (0, 1, 2)], fromCentre);
        Assert.Equal([(0, 1, 2), (1, 2, 1)], fromCorner);
    }

    private static (int Weight, int Row, int Col)[] EdgesOf(
        Dictionary<(int Row, int Col), WeightedGridNode> nodes, (int Row, int Col) cell)
        => [.. nodes[cell].Edges.Select(edge => (edge.Weight, edge.Target.Row, edge.Target.Col))];
}
