namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Gomoku, five in a row, as it is usually played: on a 15 by 15 board, a renju board's size, the
// first to make five in a row wins. The shape is the same for every variant; the rule is the
// game's. MnkGame's lines and PositionalGame play freestyle gomoku, where six or more win too.
// Standard gomoku's "exactly five" forbids a pattern rather than adding winning sets, so it is no
// positional game's rule; InARowGame, which judges runs on the board's masks, plays it as the
// ExactlyK overline rule. Renju's restrictions on the first player are not modelled.
/// <summary>Gomoku's shape: a 15 by 15 board, five in a row to win.</summary>
public readonly struct GomokuBoard : IMnkShape
{
    // Internal, so that a solution's own constant of 15 is not read as a copy of it (ARCHITECTURE
    // 17.9); Columns and Rows state it to everyone.
    internal const int Size = 15;

    /// <inheritdoc/>
    public static int Columns => Size;

    /// <inheritdoc/>
    public static int Rows => Size;

    /// <inheritdoc/>
    public static int InARow => 5;
}
