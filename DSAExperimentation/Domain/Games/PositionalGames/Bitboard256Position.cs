namespace DSAExperimentation.Domain.Games.PositionalGames;

// A position of a two-player positional game on a board of up to 256 cells, as a value: what
// BitboardPosition is on a board of at most 64, with each player's cells a Bitboard256 in place of
// a ulong. Every value is the mover's, so a move puts the new piece in the mover's mask and swaps
// the two, and whether the opponent's last claim won ends the game. It is the functional form of a
// board, a new value for every move and nothing to undo.
/// <summary>
/// A position on a board of up to 256 cells: each player's cells as a 256-bit mask, and whether
/// the game is lost.
/// </summary>
/// <param name="Mover">The cells the player to move holds.</param>
/// <param name="Opponent">The cells the other player holds.</param>
/// <param name="IsLost">
/// Whether the opponent's last claim completed a winning line, so the player to move has lost.
/// </param>
public readonly record struct Bitboard256Position(
    Bitboard256 Mover, Bitboard256 Opponent, bool IsLost)
{
    /// <summary>Gets the cells either player holds.</summary>
    public Bitboard256 Claimed => Mover | Opponent;
}
