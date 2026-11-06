namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Gomoku, five in a row, as it is usually played: on a 15 by 15 board, a renju board's size, the
// first to make five in a row wins. This is freestyle gomoku, where six or more win too. Standard
// gomoku's "exactly five" and renju's restrictions on the first player forbid patterns rather than
// add winning sets, so they are not a positional game's rules and are not modelled.
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
