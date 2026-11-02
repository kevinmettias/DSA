namespace DSAExperimentation.Domain.Games.TicTacToe;

// Tic-tac-toe as LC 1275 states it: a 3x3 board on which two players, A first, take turns marking a
// square, and the first to complete a row, a column or a diagonal wins; a full board with no line is
// a draw, and a board short of that is still being played. These are the game's *what* (ARCHITECTURE
// §17.6). How a strategy notices a completed line - rescanning the board, keeping running tallies -
// is its own *how*, and LeetCode's spelling of the answer ("A", "Draw", ...) stays with the problem.
/// <summary>The tic-tac-toe board, its turn order, the diagonals a square lies on, and how a game stands.</summary>
public static class TicTacToeRules
{
    /// <summary>The number of rows and of columns on the board, and so the length of every line.</summary>
    public const int BoardSize = 3;

    // A round is one move by each player, A's first.
    private const int MovesPerRound = 2;

    /// <summary>Names the player who makes a move: A makes the first, and the two alternate.</summary>
    /// <param name="moveIndex">The move's position in the game, from 0.</param>
    /// <returns>The player making that move.</returns>
    public static TicTacToePlayer PlayerMaking(int moveIndex)
    {
        var opensARound = moveIndex % MovesPerRound == 0;

        return opensARound ? TicTacToePlayer.A : TicTacToePlayer.B;
    }

    /// <summary>Whether a square lies on the diagonal running from the top-left corner.</summary>
    /// <param name="row">The square's row, from 0.</param>
    /// <param name="column">The square's column, from 0.</param>
    /// <returns><see langword="true"/> when the square is on that diagonal.</returns>
    public static bool IsOnDiagonal(int row, int column) => row == column;

    /// <summary>Whether a square lies on the diagonal running from the top-right corner.</summary>
    /// <param name="row">The square's row, from 0.</param>
    /// <param name="column">The square's column, from 0.</param>
    /// <returns><see langword="true"/> when the square is on that diagonal.</returns>
    public static bool IsOnAntiDiagonal(int row, int column) => row + column == BoardSize - 1;

    /// <summary>
    /// How a game stands: the player who completed a line has won, even on the board's last square;
    /// otherwise a full board is a draw and any other is still pending.
    /// </summary>
    /// <param name="winner">The player who completed a line, or <see langword="null"/> when nobody has.</param>
    /// <param name="moveCount">The number of moves made so far.</param>
    /// <returns>The game's outcome.</returns>
    public static TicTacToeOutcome OutcomeOf(TicTacToePlayer? winner, int moveCount)
    {
        if (winner is TicTacToePlayer player)
        {
            return player == TicTacToePlayer.A ? TicTacToeOutcome.AWins : TicTacToeOutcome.BWins;
        }

        var isBoardFull = moveCount == BoardSize * BoardSize;

        return isBoardFull ? TicTacToeOutcome.Draw : TicTacToeOutcome.Pending;
    }
}
