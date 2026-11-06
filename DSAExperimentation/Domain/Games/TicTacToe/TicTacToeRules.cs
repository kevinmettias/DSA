using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Domain.Games.TicTacToe;

// Tic-tac-toe as LC 1275 states it: a 3x3 board on which two players, X first, take turns marking a
// square, and the first to complete a row, a column or a diagonal wins; a full board with no line is
// a draw, and a board short of that is still being played. These are the game's *what* (ARCHITECTURE
// §17.6). How a strategy notices a completed line - rescanning the board, keeping running tallies -
// is its own *how*, and LeetCode's spelling of the answer ("A", "Draw", ...) stays with the problem.
// It is the 3,3,3 game: its board is TicTacToeBoard, its eight lines MnkGame<TicTacToeBoard>.Lines,
// and its turn order TwoPlayersAlternating with X in the first seat, which Game puts together. The
// other members are that configuration's constants and rules in the form LC 1275's strategies read.
/// <summary>The tic-tac-toe board, its turn order, the diagonals a square lies on, and how a game stands.</summary>
public static class TicTacToeRules
{
    /// <summary>The number of rows and of columns on the board, and so the length of every line.</summary>
    public const int BoardSize = TicTacToeBoard.Size;

    /// <summary>Gets tic-tac-toe as a positional game: its board, lines and turns.</summary>
    public static PositionalGame<TwoPlayersAlternating> Game { get; } =
        new(MnkGame<TicTacToeBoard>.Board, MnkGame<TicTacToeBoard>.Lines);

    /// <summary>Names the player who makes a move: X makes the first, and the two alternate.</summary>
    /// <param name="moveIndex">The move's position in the game, from 0.</param>
    /// <returns>The player making that move.</returns>
    public static TicTacToePlayer PlayerMaking(int moveIndex)
    {
        var seat = TwoPlayersAlternating.SeatMaking(moveIndex);
        var isFirstSeats = seat == TwoPlayersAlternating.First;

        return isFirstSeats ? TicTacToePlayer.X : TicTacToePlayer.O;
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
            return player == TicTacToePlayer.X ? TicTacToeOutcome.XWins : TicTacToeOutcome.OWins;
        }

        var isBoardFull = moveCount == BoardSize * BoardSize;

        return isBoardFull ? TicTacToeOutcome.Draw : TicTacToeOutcome.Pending;
    }

    /// <summary>Reads how a game of <see cref="Game"/> stands in tic-tac-toe's names.</summary>
    /// <param name="outcome">The positional game's outcome, X in the first seat.</param>
    /// <returns>The same outcome as tic-tac-toe names it.</returns>
    public static TicTacToeOutcome OutcomeOf(PositionalOutcome outcome)
    {
        if (!outcome.IsOver)
        {
            return TicTacToeOutcome.Pending;
        }

        if (outcome.Winner == PositionalOutcome.NoWinner)
        {
            return TicTacToeOutcome.Draw;
        }

        var isFirstSeats = outcome.Winner == TwoPlayersAlternating.First;

        return isFirstSeats ? TicTacToeOutcome.XWins : TicTacToeOutcome.OWins;
    }
}
