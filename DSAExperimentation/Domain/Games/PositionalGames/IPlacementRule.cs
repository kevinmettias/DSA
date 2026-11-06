namespace DSAExperimentation.Domain.Games.PositionalGames;

// Where a move puts a piece. In most positional games a player claims any open cell, so the move is
// the cell (FreePlacement). In Connect Four a player picks a column and the piece falls to the
// lowest open cell in it (Gravity). The rule is a struct witness with a static member, so a game
// played by free placement compiles to the move itself and pays nothing for gravity.
/// <summary>A placement rule: which cell a move claims on a position.</summary>
public interface IPlacementRule
{
    /// <summary>Finds the cell a move would claim.</summary>
    /// <param name="owners">
    /// The position: each cell's owner, <see cref="BoardMask.Unclaimed"/> or <see
    /// cref="BoardMask.Hole"/>.
    /// </param>
    /// <param name="move">The move, as the rule numbers moves.</param>
    /// <param name="cell">The cell it would claim, when there is one.</param>
    /// <returns>
    /// <see langword="false"/> when the move names no cell, such as a column already full.
    /// </returns>
    static abstract bool TryPlace(ReadOnlySpan<int> owners, int move, out int cell);
}
