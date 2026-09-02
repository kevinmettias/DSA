using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.GameOfLife;

// LeetCode 289. Game of Life: advance an m x n board one generation under Conway's
// rules, in place, without letting a cell's already-updated next state leak into its
// neighbors' counts (every count must read the ORIGINAL board).
//
// Both strategies solve that the same way in spirit - keep a read-only picture of the
// original board around while writing the next generation into the live one - and
// differ only in how that picture is represented: a full second board (the textbook
// answer) vs. this repo's own Set<int>, which only remembers which cells started
// live (row*cols+col encoded) instead of paying for every cell. The same "sparse Set
// instead of a full copy" move SetMatrixZeroes makes.
internal static class GameOfLifeSolution
{
    // LC 289's fixed rule constants (Conway's B3/S23), not a property of either
    // strategy.
    private const int BirthNeighborCount = 3;
    private const int SurvivalNeighborCount = 2;

    // The textbook answer: clone the board into a snapshot up front and read only the
    // snapshot while writing the next generation into the original - O(rows*cols)
    // extra space. Deliberately written without this repo's primitives - the arm the
    // Set-snapshot strategy below has to justify itself against.
    public static void AdvanceByFullBoardCopy(int[][] board)
    {
        var snapshot = Clone(board);
        var rows = board.Length;
        var cols = board[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighborsFromSnapshot(snapshot, rows, cols, r, c);
                board[r][c] =
                    liveNeighbors == BirthNeighborCount
                    || (liveNeighbors == SurvivalNeighborCount && snapshot[r][c] == 1)
                        ? 1
                        : 0;
            }
        }
    }

    // This repo's own Set<int> snapshots which cells were live in the ORIGINAL board
    // before any in-place mutation - O(live cells) extra space instead of
    // O(rows*cols), since every cell's next state only ever needs to know which
    // cells started live, never the board being written into.
    public static void AdvanceBySetSnapshot(int[][] board)
    {
        var rows = board.Length;
        var cols = board[0].Length;
        var originallyLive = SnapshotLiveCells(board, rows, cols);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var liveNeighbors = CountLiveNeighborsFromSet(originallyLive, rows, cols, r, c);
                board[r][c] =
                    liveNeighbors == BirthNeighborCount
                    || (liveNeighbors == SurvivalNeighborCount && originallyLive.Has(r * cols + c))
                        ? 1
                        : 0;
            }
        }
    }

    private static Set<int> SnapshotLiveCells(int[][] board, int rows, int cols)
    {
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

        return originallyLive;
    }

    private static int CountLiveNeighborsFromSnapshot(int[][] snapshot, int rows, int cols, int row, int col)
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

                if (r >= 0 && r < rows && c >= 0 && c < cols && snapshot[r][c] == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int CountLiveNeighborsFromSet(Set<int> originallyLive, int rows, int cols, int row, int col)
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

    private static int[][] Clone(int[][] matrix)
        => matrix.Select(row => (int[])row.Clone()).ToArray();
}
