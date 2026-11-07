namespace DSAExperimentation.Domain.Games.PositionalGames;

// Claiming any cell: the move is the cell's number. Whether the cell is open is the game's
// question, asked of every placement rule alike. On a board held as a bit mask every open cell is
// a choice, and a choice claims itself.
/// <summary>Free placement: a move names the cell it claims.</summary>
public readonly struct FreePlacement : IPlacementRule, IBitboardPlacement
{
    /// <inheritdoc/>
    public static bool TryPlace(ReadOnlySpan<int> owners, int move, out int cell)
    {
        cell = move;

        return (uint)move < (uint)owners.Length;
    }

    /// <inheritdoc/>
    public static ulong Choices(ulong open) => open;

    /// <inheritdoc/>
    public static int CellClaimed(ulong open, int choice) => choice;
}
