using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.MinimumObstacleRemovalToReachCorner.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumObstacleRemovalToReachCorner;

// LeetCode 2290. Minimum Obstacle Removal to Reach Corner: entering an obstacle
// cell costs 1, entering an empty cell costs 0 - exactly a 0/1-weighted grid, so
// this repo's own Dijkstra (ShortestPath.Dijkstra) over an ObstacleGridNode/
// ObstacleGridTopology grid finds the minimum number of obstacles removed on the
// way from the top-left to the bottom-right corner.
public sealed partial class MinimumObstacleRemovalToReachCornerTests
{
    [Fact]
    public void MinimumObstacles_LeetCodeExampleWithDetourNeeded_ReturnsTwo()
    {
        int[][] grid =
        [
            [0, 1, 1],
            [1, 1, 0],
            [1, 1, 0],
        ];

        Assert.Equal(2, MinimumObstacles(grid));
    }

    [Fact]
    public void MinimumObstacles_ClearPathAroundOneObstacle_ReturnsZero()
    {
        int[][] grid =
        [
            [0, 0, 0],
            [0, 1, 0],
            [0, 0, 0],
        ];

        Assert.Equal(0, MinimumObstacles(grid));
    }

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int MinimumObstacles(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var nodes = new ObstacleGridNode[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                nodes[row, col] = new ObstacleGridNode(row, col);
            }
        }

        WireEdges(nodes, grid, rows, cols);

        var distances = ShortestPath.Dijkstra<
            ObstacleGridNode, ObstacleGridTopology, ListEdges<ObstacleGridNode, int>, int>(nodes[0, 0]);

        return distances[nodes[rows - 1, cols - 1]];
    }

    private readonly record struct ObstacleGridWiring(ObstacleGridNode[,] Nodes, int[][] Grid, int Rows, int Cols);

    private static void WireEdges(ObstacleGridNode[,] nodes, int[][] grid, int rows, int cols)
    {
        var wiring = new ObstacleGridWiring(nodes, grid, rows, cols);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                foreach (var direction in Directions)
                {
                    TryAddEdge(wiring, row, col, direction);
                }
            }
        }
    }

    private static void TryAddEdge(ObstacleGridWiring wiring, int row, int col, (int DRow, int DCol) direction)
    {
        var r = row + direction.DRow;
        var c = col + direction.DCol;

        if (r < 0 || r >= wiring.Rows || c < 0 || c >= wiring.Cols)
        {
            return;
        }

        wiring.Nodes[row, col].Edges.Add((wiring.Grid[r][c], wiring.Nodes[r, c]));
    }
}
