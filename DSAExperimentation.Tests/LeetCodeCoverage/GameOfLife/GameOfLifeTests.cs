using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GameOfLife;

// LeetCode 289. Game of Life: this repo's own Set<int> snapshots which cells were
// live in the ORIGINAL board (row*cols+col encoded) before any in-place mutation -
// the same "sparse Set instead of a full second copy" move SetMatrixZeroesTests.cs
// already makes. Every cell's next state only ever needs the original board to
// count neighbors, never the board being written into, so tracking just the live
// cells is enough - O(live cells) extra space instead of O(rows*cols).
public sealed class GameOfLifeTests
{
    [Fact]
    public void Advance_LeetCodeExample_MatchesExpectedNextGeneration()
    {
        int[][] board = [[0, 1, 0], [0, 0, 1], [1, 1, 1], [0, 0, 0]];

        Advance(board);

        Assert.Equal([[0, 0, 0], [1, 0, 1], [0, 1, 1], [0, 1, 0]], board);
    }

    [Fact]
    public void Advance_TwoByTwoLiveBlock_StaysStable()
    {
        int[][] board = [[1, 1], [1, 1]];

        Advance(board);

        Assert.Equal([[1, 1], [1, 1]], board);
    }

    private static void Advance(int[][] board)
    {
        var rows = board.Length;
        var cols = board[0].Length;
        var originallyLive = new Set<int>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (board[r][c] == 1)
                {
                    originallyLive.TryAdd(r * cols + c);
                }
            }
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighbors(originallyLive, rows, cols, r, c);
                board[r][c] = liveNeighbors == 3 || (liveNeighbors == 2 && originallyLive.Has(r * cols + c)) ? 1 : 0;
            }
        }
    }

    private static int CountLiveNeighbors(Set<int> originallyLive, int rows, int cols, int row, int col)
    {
        var count = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                {
                    continue;
                }

                var r = row + dr;
                var c = col + dc;

                if (r >= 0 && r < rows && c >= 0 && c < cols && originallyLive.Has(r * cols + c))
                {
                    count++;
                }
            }
        }

        return count;
    }
}
