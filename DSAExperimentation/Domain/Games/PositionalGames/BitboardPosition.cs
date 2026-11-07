namespace DSAExperimentation.Domain.Games.PositionalGames;

// A position of a two-player positional game on a board of at most 64 cells, as a value: the
// cells the player to move holds and the cells the opponent holds, each a bit mask with bit c for
// cell c, and whether the opponent's last claim completed a winning set, which ends the game.
// Every value is the mover's, so a move puts the new piece in the mover's mask and swaps the two:
// the player who just moved becomes the opponent. It is the functional form of a board, a new
// value for every move and nothing to undo, and two masks are what a search copies per move.
/// <summary>
/// A position on a board of at most 64 cells: each player's cells as a bit mask, and whether the
/// game is lost.
/// </summary>
/// <param name="Mover">The cells the player to move holds.</param>
/// <param name="Opponent">The cells the other player holds.</param>
/// <param name="IsLost">
/// Whether the opponent's last claim completed a winning set, so the player to move has lost.
/// </param>
public readonly record struct BitboardPosition(ulong Mover, ulong Opponent, bool IsLost)
{
    /// <summary>Gets the cells either player holds.</summary>
    public ulong Claimed => Mover | Opponent;

    /// <summary>Makes the bit mask of a list of cells.</summary>
    /// <param name="cells">Cell numbers below 64.</param>
    /// <returns>The mask with a bit set at every listed cell.</returns>
    public static ulong MaskOf(ReadOnlySpan<int> cells)
    {
        var mask = 0UL;

        foreach (var cell in cells)
        {
            mask |= 1UL << cell;
        }

        return mask;
    }
}
