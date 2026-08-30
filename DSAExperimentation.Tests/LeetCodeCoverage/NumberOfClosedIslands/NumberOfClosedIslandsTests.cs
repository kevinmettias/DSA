using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfClosedIslands;

// LeetCode 1254. Number of Closed Islands: the same border-flood-fill DFS shape
// NumberOfIslandsTests/MaxAreaOfIslandTests/SurroundedRegionsTests already use
// over DepthFirstSearch.Traverse, except a land component only counts when NONE
// of its visited cells sit on the grid's own edge - the same interior-vs-border
// distinction SurroundedRegionsTests' Mark(border 'O') makes, just counting
// components instead of relabeling them.
public sealed partial class NumberOfClosedIslandsTests
{
    [Fact]
    public void ClosedIslandCount_ClassicExample_CountsTwoInteriorComponents()
    {
        int[][] grid =
        [
            [1, 1, 1, 1, 1, 1, 1, 0],
            [1, 0, 0, 0, 0, 1, 1, 0],
            [1, 0, 1, 0, 1, 1, 1, 0],
            [1, 0, 0, 0, 0, 1, 0, 1],
            [1, 1, 1, 1, 1, 1, 1, 0],
        ];

        Assert.Equal(2, ClosedIslandCount(grid));
    }

    [Fact]
    public void ClosedIslandCount_LandComponentTouchesBorder_ExcludedFromCount()
    {
        int[][] grid =
        [
            [0, 1, 1, 1, 1],
            [1, 0, 0, 1, 1],
            [1, 0, 0, 1, 1],
            [1, 1, 1, 1, 1],
            [1, 1, 1, 1, 1],
        ];

        // (0,0) is its own land component touching the top-left border and must
        // not be counted; the interior 2x2 block at rows/cols 1-2 is the only
        // closed island.
        Assert.Equal(1, ClosedIslandCount(grid));
    }

    private static int ClosedIslandCount(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), Neighbors);
                var touchesBorder = false;

                foreach (var (row, col) in island)
                {
                    if (row == 0 || row == rows - 1 || col == 0 || col == cols - 1)
                    {
                        touchesBorder = true;
                    }

                    grid[row][col] = 1;
                }

                if (!touchesBorder)
                {
                    count++;
                }
            }
        }

        return count;

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            (int Row, int Col)[] next =
                [(p.Row + 1, p.Col), (p.Row - 1, p.Col), (p.Row, p.Col + 1), (p.Row, p.Col - 1)];

            foreach (var n in next)
            {
                if (n.Row >= 0 && n.Row < rows && n.Col >= 0 && n.Col < cols && grid[n.Row][n.Col] == 0)
                {
                    yield return n;
                }
            }
        }
    }
}
