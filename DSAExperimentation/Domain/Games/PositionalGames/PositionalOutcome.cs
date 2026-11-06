namespace DSAExperimentation.Domain.Games.PositionalGames;

// How a positional game stands: still being played, over with nobody winning, or won by one seat.
// The three are made only by the members below, so a winner is never read off a game that is
// still going.
/// <summary>How a positional game stands: pending, drawn, or won by a seat.</summary>
public readonly record struct PositionalOutcome
{
    /// <summary>The <see cref="Winner"/> of a game nobody has won.</summary>
    public const int NoWinner = -1;

    /// <summary>Gets the outcome of a game still being played.</summary>
    public static PositionalOutcome Pending { get; } = new() { Winner = NoWinner };

    /// <summary>Gets the outcome of a game over with nobody winning.</summary>
    public static PositionalOutcome Drawn { get; } = new() { IsOver = true, Winner = NoWinner };

    /// <summary>Gets a value indicating whether the game is over, won or drawn.</summary>
    public bool IsOver { get; private init; }

    /// <summary>Gets the seat that won, or <see cref="NoWinner"/>.</summary>
    public int Winner { get; private init; }

    /// <summary>Makes the outcome of a game won by one seat.</summary>
    /// <param name="seat">The winner's seat, from 0.</param>
    /// <returns>The outcome.</returns>
    public static PositionalOutcome WonBy(int seat) => new() { IsOver = true, Winner = seat };
}
