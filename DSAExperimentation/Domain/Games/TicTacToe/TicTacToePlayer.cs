namespace DSAExperimentation.Domain.Games.TicTacToe;

// The game's own names for its players: X, who moves first, and O. LC 1275 calls them "A" and "B";
// that is its spelling, kept in its folder with its "Draw", not the game's (ARCHITECTURE §17.6).
/// <summary>One of the two tic-tac-toe players.</summary>
public enum TicTacToePlayer
{
    /// <summary>X, the player who makes the first move.</summary>
    X,

    /// <summary>O, the player who makes the second move.</summary>
    O,
}
