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

    private readonly record struct GridDimensions(int Rows, int Cols);

    private static void Advance(int[][] board)
    {
        var dimensions = new GridDimensions(board.Length, board[0].Length);
        var originallyLive = SnapshotLiveCells(board, dimensions);
        ApplyNextGeneration(board, originallyLive, dimensions);
    }

    private static Set<int> SnapshotLiveCells(int[][] board, GridDimensions dimensions)
    {
        var originallyLive = new Set<int>();

        for (var r = 0; r < dimensions.Rows; r++)
        {
            for (var c = 0; c < dimensions.Cols; c++)
            {
                if (board[r][c] == 1)
                {
                    originallyLive.TryAdd(r * dimensions.Cols + c);
                }
            }
        }

        return originallyLive;
    }

    private static void ApplyNextGeneration(int[][] board, Set<int> originallyLive, GridDimensions dimensions)
    {
        for (var r = 0; r < dimensions.Rows; r++)
        {
            for (var c = 0; c < dimensions.Cols; c++)
            {
                var liveNeighbors = CountLiveNeighbors(originallyLive, dimensions, r, c);
                board[r][c] =
                    liveNeighbors == 3 || (liveNeighbors == 2 && originallyLive.Has(r * dimensions.Cols + c))
                        ? 1
                        : 0;
            }
        }
    }

    private static int CountLiveNeighbors(Set<int> originallyLive, GridDimensions dimensions, int row, int col)
    {
        var count = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (IsLiveNeighbor(originallyLive, dimensions, (row, col), (dr, dc)))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsLiveNeighbor(
        Set<int> originallyLive, GridDimensions dimensions, (int Row, int Col) cell, (int DRow, int DCol) offset)
    {
        if (offset.DRow == 0 && offset.DCol == 0)
        {
            return false;
        }

        var r = cell.Row + offset.DRow;
        var c = cell.Col + offset.DCol;

        return r >= 0 && r < dimensions.Rows && c >= 0 && c < dimensions.Cols
            && originallyLive.Has(r * dimensions.Cols + c);
    }
}
