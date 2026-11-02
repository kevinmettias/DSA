using DSAExperimentation.Domain.Games.TicTacToe;
using Vocabulary = DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame.TicTacToeVocabulary;

namespace DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

// LeetCode 1275. Find Winner on a Tic Tac Toe Game: replay the alternating moves
// and report "A"/"B" for a completed line, "Draw" once every square is taken, and
// "Pending" while squares remain.
//
// No repo primitive fits either strategy - Grid/GridNode model pathfinding
// adjacency for graph walks, not line occupancy - so the two arms differ only in
// how much of the board each move re-examines: rebuild and rescan every row,
// column and diagonal, or keep running +/-1 tallies and touch only the lines the
// new move actually falls on. Both play LC 1275's fixed 3 x 3 board. The game's
// rules are Domain/Games/TicTacToe's, which the running-counts arm reads; the
// board-rescan baseline keeps its own copy of them (ARCHITECTURE §17.5).
internal static class FindWinnerOnATicTacToeGameSolution
{
    // The textbook baseline: keep the board itself and, after each move, rescan
    // every row, every column and both diagonals from scratch. Plain BCL arrays
    // and loops - this is what you would write without any bookkeeping insight.
    public static string FindWinnerByBoardRescan(int[][] moves)
    {
        var board = new int[Vocabulary.BoardSize, Vocabulary.BoardSize];

        for (var i = 0; i < moves.Length; i++)
        {
            board[moves[i][0], moves[i][1]] = MarkFor(i);

            var winner = ScanAllLines(board, Vocabulary.BoardSize);

            if (winner != Vocabulary.Empty)
            {
                return PlayerFor(winner);
            }
        }

        return Outcome(moves.Length, Vocabulary.BoardSize);
    }

    private static int ScanAllLines(int[,] board, int boardSize)
    {
        var rowWinner = ScanRows(board, boardSize);
        var columnWinner = rowWinner != Vocabulary.Empty ? rowWinner : ScanColumns(board, boardSize);

        return columnWinner != Vocabulary.Empty ? columnWinner : ScanDiagonals(board, boardSize);
    }

    // Every row's line, one at a time, until one of them comes back decided.
    private static int ScanRows(int[,] board, int boardSize)
    {
        for (var row = 0; row < boardSize; row++)
        {
            var winner = LineWinner(board, boardSize, new LineScan(row, 0, 0, 1));

            if (winner != Vocabulary.Empty)
            {
                return winner;
            }
        }

        return Vocabulary.Empty;
    }

    // Every column's line, same shape as the row scan above.
    private static int ScanColumns(int[,] board, int boardSize)
    {
        for (var column = 0; column < boardSize; column++)
        {
            var winner = LineWinner(board, boardSize, new LineScan(0, column, 1, 0));

            if (winner != Vocabulary.Empty)
            {
                return winner;
            }
        }

        return Vocabulary.Empty;
    }

    // Both diagonals last, since neither belongs to a row or a column.
    private static int ScanDiagonals(int[,] board, int boardSize)
    {
        var diagonal = LineWinner(board, boardSize, new LineScan(0, 0, 1, 1));

        return diagonal != Vocabulary.Empty
            ? diagonal
            : LineWinner(board, boardSize, new LineScan(0, boardSize - 1, 1, -1));
    }

    private static int LineWinner(int[,] board, int boardSize, LineScan line)
    {
        var first = board[line.StartRow, line.StartColumn];

        if (first == Vocabulary.Empty)
        {
            return Vocabulary.Empty;
        }

        for (var i = 1; i < boardSize; i++)
        {
            if (board[line.StartRow + (i * line.RowStep), line.StartColumn + (i * line.ColumnStep)] != first)
            {
                return Vocabulary.Empty;
            }
        }

        return first;
    }

    // A line to scan for a winner: starting cell plus per-step row/column delta.
    private readonly record struct LineScan(int StartRow, int StartColumn, int RowStep, int ColumnStep);

    // Running per-row, per-column and per-diagonal tallies moved by +1 for A and
    // -1 for B: a line is won the instant its tally reaches +/-BoardSize, so each
    // move costs O(1) instead of a full board rescan. Who moves, which diagonals a
    // square lies on and how the game stands come from Domain/Games/TicTacToe; the signed
    // tally is this strategy's own.
    public static string FindWinnerByRunningCounts(int[][] moves)
    {
        var tallies = new LineTallies();

        for (var i = 0; i < moves.Length; i++)
        {
            var player = TicTacToeRules.PlayerMaking(i);

            if (tallies.HasCompletedLine(moves[i][0], moves[i][1], player))
            {
                var won = TicTacToeRules.OutcomeOf(player, i + 1);

                return Vocabulary.AnswerFor(won);
            }
        }

        var unwon = TicTacToeRules.OutcomeOf(null, moves.Length);

        return Vocabulary.AnswerFor(unwon);
    }

    private sealed class LineTallies
    {
        private readonly int[] _rowCount = new int[TicTacToeRules.BoardSize];
        private readonly int[] _columnCount = new int[TicTacToeRules.BoardSize];
        private int _diagonal;
        private int _antiDiagonal;

        public bool HasCompletedLine(int row, int column, TicTacToePlayer player)
        {
            var mark = player == TicTacToePlayer.A ? Vocabulary.MarkA : Vocabulary.MarkB;

            _rowCount[row] += mark;
            _columnCount[column] += mark;
            _diagonal += TicTacToeRules.IsOnDiagonal(row, column) ? mark : 0;
            _antiDiagonal += TicTacToeRules.IsOnAntiDiagonal(row, column) ? mark : 0;

            return Math.Abs(_rowCount[row]) == TicTacToeRules.BoardSize
                || Math.Abs(_columnCount[column]) == TicTacToeRules.BoardSize
                || Math.Abs(_diagonal) == TicTacToeRules.BoardSize
                || Math.Abs(_antiDiagonal) == TicTacToeRules.BoardSize;
        }
    }

    // Player A takes every even-indexed move, player B every odd one.
    private static int MarkFor(int moveIndex) =>
        IsPlayerATurn(moveIndex) ? Vocabulary.MarkA : Vocabulary.MarkB;

    // Named because it is a question the alternation above keeps asking, not a quantity
    // the board itself ever holds.
    private static bool IsPlayerATurn(int moveIndex) => moveIndex % Vocabulary.PlayerCount == 0;

    private static string PlayerFor(int mark) =>
        mark == Vocabulary.MarkA ? Vocabulary.PlayerA : Vocabulary.PlayerB;

    // With no line completed, the game is a draw once every square is taken and
    // pending while any remain.
    private static string Outcome(int moveCount, int boardSize) =>
        IsBoardFull(moveCount, boardSize) ? Vocabulary.Draw : Vocabulary.Pending;

    private static bool IsBoardFull(int moveCount, int boardSize) => moveCount == boardSize * boardSize;
}
