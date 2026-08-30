using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToMakeAtLeastOneValidPathInAGrid;

// LeetCode 1368. Minimum Cost to Make at Least One Valid Path in a Grid: each
// cell's arrow (1=right, 2=left, 3=down, 4=up) is free to follow and costs 1 to
// override - exactly a 0/1-weighted shortest path from (0,0) to the far corner,
// the same Dijkstra-shaped grid frontier SwimInRisingWaterTests already proves
// (this repo's own Heap<Element,TOrder>, ordered by ByPriorityOrder<TNode,
// TWeight>), just with a summed cost instead of a minimax one, and settle-once
// bookkeeping (a distances map plus a settled set, mirroring ShortestPath.cs's
// own Explore loop) since a plain "mark visited when pushed" shortcut is only
// sound for minimax costs, not additive ones.
public sealed partial class MinimumCostToMakeAtLeastOneValidPathInAGridTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    [Fact]
    public void MinCost_ArrowsAlreadyFormAValidPath_ReturnsZero()
    {
        int[][] grid = [[1, 1, 3], [3, 2, 2], [1, 1, 4]];

        Assert.Equal(0, MinCost(grid));
    }

    [Fact]
    public void MinCost_OneArrowMustBeOverridden_ReturnsOne()
    {
        int[][] grid = [[1, 2], [4, 3]];

        Assert.Equal(1, MinCost(grid));
    }

    [Fact]
    public void MinCost_SingleCellGrid_ReturnsZero()
    {
        int[][] grid = [[1]];

        Assert.Equal(0, MinCost(grid));
    }

    private static int MinCost(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;

            if (!settled.Add((row, col)))
            {
                continue;
            }

            if (row == rows - 1 && col == cols - 1)
            {
                return entry.Priority;
            }

            for (var direction = 0; direction < Directions.Length; direction++)
            {
                var (dRow, dCol) = Directions[direction];
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                var weight = grid[row][col] == direction + 1 ? 0 : 1;
                var candidate = distances[(row, col)] + weight;

                if (!distances.TryGetValue((nextRow, nextCol), out var known) || candidate < known)
                {
                    distances[(nextRow, nextCol)] = candidate;
                    frontier.Push(((nextRow, nextCol), candidate));
                }
            }
        }

        throw new InvalidOperationException("Unreachable for a valid grid: every cell is orthogonally reachable from (0,0).");
    }
}
