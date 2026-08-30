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
        var totalLand = grid.Sum(row => row.Sum());

        if (totalLand <= 1)
        {
            return 0;
        }

        if (CountIslandsExcluding(grid, skipRow: -1, skipCol: -1) != 1)
        {
            return 0;
        }

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1 && CountIslandsExcluding(grid, r, c) != 1)
                {
                    return 1;
                }
            }
        }

        return 2;
    }

    private static int CountIslandsExcluding(int[][] grid, int skipRow, int skipCol)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var visited = new bool[rows, cols];
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 1 || visited[r, c] || (r == skipRow && c == skipCol))
                {
                    continue;
                }

                count++;

                foreach (var (row, col) in DepthFirstSearch.Traverse((r, c), Neighbors))
                {
                    visited[row, col] = true;
                }
            }
        }

        return count;

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != 1 || (nextRow == skipRow && nextCol == skipCol))
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }
}
