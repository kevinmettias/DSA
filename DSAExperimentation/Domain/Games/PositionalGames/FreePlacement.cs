namespace DSAExperimentation.Domain.Games.PositionalGames;

// Claiming any cell: the move is the cell's number. Whether the cell is open is the game's
// question, asked of every placement rule alike.
/// <summary>Free placement: a move names the cell it claims.</summary>
public readonly struct FreePlacement : IPlacementRule
{
    /// <inheritdoc/>
    public static bool TryPlace(ReadOnlySpan<int> owners, int move, out int cell)
    {
        cell = move;

        return (uint)move < (uint)owners.Length;
    }
}
