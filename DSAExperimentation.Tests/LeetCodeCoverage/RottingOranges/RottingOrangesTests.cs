namespace DSAExperimentation.Tests.LeetCodeCoverage.RottingOranges;

// LeetCode 994. Rotting Oranges: multi-source BFS seeded with every already-rotten
// orange at once (minute 0), the same shape ZeroOneMatrixTests already uses for its
// multi-source distance sweep - just tracking minutes-to-rot per fresh cell instead
// of distance-to-nearest-zero, using this repo's own Queue<TElement> as the FIFO
// frontier.
public sealed partial class RottingOrangesTests
{
    [Fact]
    public void OrangesRotting_ClassicExample_ReturnsMinutesUntilAllRot()
    {
        int[][] grid = [[2, 1, 1], [1, 1, 0], [0, 1, 1]];

        Assert.Equal(4, OrangesRotting(grid));
    }

    [Fact]
    public void OrangesRotting_UnreachableFreshOrange_ReturnsNegativeOne()
    {
        int[][] grid = [[2, 1, 1], [0, 1, 1], [1, 0, 1]];

        Assert.Equal(-1, OrangesRotting(grid));
    }

    [Fact]
    public void OrangesRotting_NoFreshOranges_ReturnsZero()
    {
        int[][] grid = [[0, 2]];

        Assert.Equal(0, OrangesRotting(grid));
    }

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int OrangesRotting(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var minutesToRot = new int[rows][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        var freshCount = 0;

        for (var r = 0; r < rows; r++)
        {
            minutesToRot[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] == 2)
                {
                    frontier.Enqueue((r, c));
                }
                else if (grid[r][c] == 1)
                {
                    freshCount++;
                }
            }
        }

        var minutesElapsed = 0;

        while (frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                grid[nextRow][nextCol] = 2;
                minutesToRot[nextRow][nextCol] = minutesToRot[cell.Row][cell.Col] + 1;
                freshCount--;
                minutesElapsed = Math.Max(minutesElapsed, minutesToRot[nextRow][nextCol]);
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return freshCount == 0 ? minutesElapsed : -1;
    }
}
