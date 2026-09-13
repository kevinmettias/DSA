namespace DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

// LeetCode 1275. Find Winner on a Tic Tac Toe Game: replay the alternating moves
// and report "A"/"B" for a completed line, "Draw" once every square is taken, and
// "Pending" while squares remain.
//
// No repo primitive fits either strategy - Grid/GridNode model pathfinding
// adjacency for graph walks, not line occupancy - so the two arms differ only in
// how much of the board each move re-examines: rebuild and rescan every row,
// column and diagonal, or keep running +/-1 tallies and touch only the lines the
// new move actually falls on. Both are generalized over board size so a benchmark
// can scale them; LC 1275 itself fixes the board at BoardSize.
internal static class FindWinnerOnATicTacToeGameSolution
{
    // LC 1275's board is 3 x 3, and a line is three marks long.
    public const int BoardSize = 3;

    private const string PlayerA = "A";
    private const string PlayerB = "B";
    private const string Draw = "Draw";
    private const string Pending = "Pending";
    private const int PlayerCount = 2;
    private const int Empty = 0;
    private const int MarkA = 1;
    private const int MarkB = -1;

    // The textbook baseline: keep the board itself and, after each move, rescan
    // every row, every column and both diagonals from scratch. Plain BCL arrays
    // and loops - this is what you would write without any bookkeeping insight.
    public static string FindWinnerByBoardRescan(int[][] moves) =>
        FindWinnerByBoardRescan(moves, BoardSize);

    public static string FindWinnerByBoardRescan(int[][] moves, int boardSize)
    {
        var board = new int[boardSize, boardSize];

        for (var i = 0; i < moves.Length; i++)
        {
            board[moves[i][0], moves[i][1]] = MarkFor(i);

            var winner = ScanAllLines(board, boardSize);

            if (winner != Empty)
            {
                return PlayerFor(winner);
            }
        }

        return Outcome(moves.Length, boardSize);
    }

    private static int ScanAllLines(int[,] board, int boardSize)
    {
        for (var row = 0; row < boardSize; row++)
        {
            var winner = LineWinner(board, boardSize, new LineScan(row, 0, 0, 1));

            if (winner != Empty)
            {
                return winner;
            }
        }

        for (var column = 0; column < boardSize; column++)
        {
            var winner = LineWinner(board, boardSize, new LineScan(0, column, 1, 0));

            if (winner != Empty)
            {
                return winner;
            }
        }

        var diagonal = LineWinner(board, boardSize, new LineScan(0, 0, 1, 1));

        return diagonal != Empty
            ? diagonal
            : LineWinner(board, boardSize, new LineScan(0, boardSize - 1, 1, -1));
    }

    private static int LineWinner(int[,] board, int boardSize, LineScan line)
    {
        var first = board[line.StartRow, line.StartColumn];

        if (first == Empty)
        {
            return Empty;
        }

        for (var i = 1; i < boardSize; i++)
        {
            if (board[line.StartRow + (i * line.RowStep), line.StartColumn + (i * line.ColumnStep)] != first)
            {
                return Empty;
            }
        }

        return first;
    }

    // A line to scan for a winner: starting cell plus per-step row/column delta.
    private readonly record struct LineScan(int StartRow, int StartColumn, int RowStep, int ColumnStep);

    // Running per-row, per-column and per-diagonal tallies moved by +1 for A and
    // -1 for B: a line is won the instant its tally reaches +/-boardSize, so each
    // move costs O(1) instead of a full board rescan.
    public static string FindWinnerByRunningCounts(int[][] moves) =>
        FindWinnerByRunningCounts(moves, BoardSize);

    public static string FindWinnerByRunningCounts(int[][] moves, int boardSize)
    {
        var tallies = new LineTallies(boardSize);

        for (var i = 0; i < moves.Length; i++)
        {
            var mark = MarkFor(i);

            if (tallies.Apply(moves[i][0], moves[i][1], mark))
            {
                return PlayerFor(mark);
            }
        }

        return Outcome(moves.Length, boardSize);
    }

    private sealed class LineTallies(int boardSize)
    {
        private readonly int[] _rowCount = new int[boardSize];
        private readonly int[] _columnCount = new int[boardSize];
        private int _diagonal;
        private int _antiDiagonal;

        public bool Apply(int row, int column, int mark)
        {
            _rowCount[row] += mark;
            _columnCount[column] += mark;
            _diagonal += row == column ? mark : 0;
            _antiDiagonal += row + column == boardSize - 1 ? mark : 0;

            return Math.Abs(_rowCount[row]) == boardSize
                || Math.Abs(_columnCount[column]) == boardSize
                || Math.Abs(_diagonal) == boardSize
                || Math.Abs(_antiDiagonal) == boardSize;
        }
    }

    // Player A takes every even-indexed move, player B every odd one.
    private static int MarkFor(int moveIndex) => moveIndex % PlayerCount == 0 ? MarkA : MarkB;

    private static string PlayerFor(int mark) => mark == MarkA ? PlayerA : PlayerB;

    // With no line completed, the game is a draw once every square is taken and
    // pending while any remain.
    private static string Outcome(int moveCount, int boardSize) =>
        moveCount == boardSize * boardSize ? Draw : Pending;
}
