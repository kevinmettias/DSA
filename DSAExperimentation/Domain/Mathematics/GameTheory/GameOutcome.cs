namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// What a position of a two-player game is worth to the player to move when the game is only won,
// lost or drawn: a win when that player can force one, a loss when the opponent can, and a draw
// when neither can, as when best play goes round a cycle for ever. The values are the signs of
// IAlternatingGame's win-or-lose values, so the outcome a move leaves the opponent is the negation
// of the one it brings its mover, and a draw is the default a solver starts every position from.
/// <summary>The outcome of a position for the player to move: a loss, a draw or a win.</summary>
public enum GameOutcome : sbyte
{
    /// <summary>The opponent can force a win.</summary>
    Loss = -1,

    /// <summary>Neither player can force a win.</summary>
    Draw = 0,

    /// <summary>The player to move can force a win.</summary>
    Win = 1,
}
