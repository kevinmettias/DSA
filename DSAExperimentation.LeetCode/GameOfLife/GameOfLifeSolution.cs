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
                var liveNeighbors = CountLiveNeighborsFromSnapshot(snapshot, (r, c));
                board[r][c] = IsAliveNextGeneration(liveNeighbors, snapshot[r][c]) ? 1 : 0;
            }
        }
    }

    // The snapshot is the whole board, so its own dimensions already say how far a
    // cell's neighbourhood reaches; only the cell being counted has to be handed over.
    private static int CountLiveNeighborsFromSnapshot(int[][] snapshot, (int Row, int Col) cell)
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

                var r = cell.Row + dr;
                var c = cell.Col + dc;

                if (IsInside(r, c, snapshot.Length, snapshot[0].Length) && snapshot[r][c] == 1)
                {
                    count++;
                }
            }
        }

        return count;
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
                var liveNeighbors = CountLiveNeighborsFromSet(originallyLive, rows, cols, (r, c));
                var wasLive = originallyLive.Has(r * cols + c) ? 1 : 0;
                board[r][c] = IsAliveNextGeneration(liveNeighbors, wasLive) ? 1 : 0;
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

    // This snapshot is a Set of encoded cells, so it cannot say how wide the board is
    // and the two bounds stay; the cell they locate travels as the one thing they say.
    private static int CountLiveNeighborsFromSet(
        Set<int> originallyLive, int rows, int cols, (int Row, int Col) cell)
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

                var r = cell.Row + dr;
                var c = cell.Col + dc;

                if (IsInside(r, c, rows, cols) && originallyLive.Has(r * cols + c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Conway's two rules are the definition of a cell's next state, so they get a name and
    // the loop body reads as "write the next state" instead of applying && before || in
    // its head. The current state is an int rather than a bool because it is the board's
    // own 0/1, which keeps the second strategy's Set membership from needing a conversion
    // at a call site whose meaning would then move into the argument's position.
    private static bool IsAliveNextGeneration(int liveNeighbors, int currentState)
        => liveNeighbors == BirthNeighborCount
           || (liveNeighbors == SurvivalNeighborCount && currentState == 1);

    // Both coordinates within the board is one idea, and it was written out twice.
    private static bool IsInside(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    private static int[][] Clone(int[][] matrix)
        => matrix.Select(row => (int[])row.Clone()).ToArray();
}
