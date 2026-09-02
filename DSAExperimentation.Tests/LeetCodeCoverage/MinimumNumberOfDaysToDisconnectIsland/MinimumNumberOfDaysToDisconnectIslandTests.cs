using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfDaysToDisconnectIsland;

// LeetCode 1568. Minimum Number of Days to Disconnect Island: the answer is always
// 0, 1, or 2 - already disconnected (0 or 1 land cell, or 2+ components), disconnect-
// able by removing one cell (an articulation cell), or otherwise 2 (remove any land
// cell, then any land cell adjacent to what's left). Each "how many components?"
// check is a connected-components count over the grid, the same
// DepthFirstSearch.Traverse-over-4-directional-neighbors shape NumberOfIslandsTests
// already uses, just parameterized by one temporarily-removed cell instead of
// mutating the grid.
public sealed partial class MinimumNumberOfDaysToDisconnectIslandTests
{
    [Fact]
    public void MinDays_AlreadyTwoComponents_ReturnsZero()
        => Assert.Equal(0, MinDays([[1, 0], [0, 1]]));

    [Fact]
    public void MinDays_SingleLandCell_ReturnsZero()
        => Assert.Equal(0, MinDays([[1]]));

    [Fact]
    public void MinDays_StraightLineOfThree_ReturnsOneViaMiddleArticulationCell()
        => Assert.Equal(1, MinDays([[1, 1, 1]]));

    [Fact]
    public void MinDays_SolidTwoByTwoBlock_ReturnsTwoNoSingleCellDisconnects()
        => Assert.Equal(2, MinDays([[0, 1, 1, 0], [0, 1, 1, 0], [0, 0, 0, 0]]));

    [Fact]
    public void MinDays_StraightLineOfTwo_ReturnsTwo()
        => Assert.Equal(2, MinDays([[1, 1]]));

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private static int MinDays(int[][] grid)
    {
        if (IsTriviallyDisconnected(grid))
        {
            return 0;
        }

        if (HasArticulationCell(grid))
        {
            return 1;
        }

        return 2;
    }

    private static bool IsTriviallyDisconnected(int[][] grid)
    {
        var totalLand = grid.Sum(row => row.Sum());

        return totalLand <= 1 || CountIslandsExcluding(grid, skipRow: -1, skipCol: -1) != 1;
    }

    private static bool HasArticulationCell(int[][] grid)
    {
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1 && CountIslandsExcluding(grid, r, c) != 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private readonly record struct IslandScan(int[][] Grid, int Rows, int Cols, int SkipRow, int SkipCol);

    private static int CountIslandsExcluding(int[][] grid, int skipRow, int skipCol)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var visited = new bool[rows, cols];
        var scan = new IslandScan(grid, rows, cols, skipRow, skipCol);

        return CountIslands(scan, visited);
    }

    private static int CountIslands(IslandScan scan, bool[,] visited)
    {
        var count = 0;

        for (var r = 0; r < scan.Rows; r++)
        {
            for (var c = 0; c < scan.Cols; c++)
            {
                if (scan.Grid[r][c] != 1 || visited[r, c] || (r == scan.SkipRow && c == scan.SkipCol))
                {
                    continue;
                }

                count++;
                MarkIsland(scan, visited, (r, c));
            }
        }

        return count;
    }

    private static void MarkIsland(IslandScan scan, bool[,] visited, (int Row, int Col) start)
    {
        foreach (var (row, col) in DepthFirstSearch.Traverse(start, p => GetNeighbors(scan, p)))
        {
            visited[row, col] = true;
        }
    }

    private static IEnumerable<(int Row, int Col)> GetNeighbors(IslandScan scan, (int Row, int Col) p)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            if (TryGetNeighbor(scan, p, (dRow, dCol), out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static bool TryGetNeighbor(
        IslandScan scan, (int Row, int Col) p, (int DRow, int DCol) offset, out (int Row, int Col) neighbor)
    {
        var nextRow = p.Row + offset.DRow;
        var nextCol = p.Col + offset.DCol;

        if (nextRow < 0 || nextRow >= scan.Rows || nextCol < 0 || nextCol >= scan.Cols)
        {
            neighbor = default;
            return false;
        }

        if (scan.Grid[nextRow][nextCol] != 1 || (nextRow == scan.SkipRow && nextCol == scan.SkipCol))
        {
            neighbor = default;
            return false;
        }

        neighbor = (nextRow, nextCol);
        return true;
    }
}
