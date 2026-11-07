namespace DSAExperimentation.Domain.Games.PositionalGames;

// A placement rule read on a board of at most 64 cells held as a bit mask, bit c for cell c: the
// same rule as IPlacementRule, written for a position that is two masks rather than an array of
// owners. A move is chosen by a choice the rule names (a cell, or a column under gravity), and
// the choices open on a position are the set bits of one mask, so a solver numbers its moves by
// counting them, lowest first, and allocates nothing. Static members, as IPlacementRule's are, so
// a game compiles the rule it is played by into its own code.
/// <summary>
/// A placement rule on a board held as a bit mask: the choices open, and the cell each claims.
/// </summary>
public interface IBitboardPlacement
{
    /// <summary>Finds the choices open on a position, one bit each.</summary>
    /// <param name="open">The open cells: on the board and claimed by nobody.</param>
    /// <returns>The open choices, each a bit at the choice's number.</returns>
    static abstract ulong Choices(ulong open);

    /// <summary>Finds the cell an open choice claims.</summary>
    /// <param name="open">The open cells: on the board and claimed by nobody.</param>
    /// <param name="choice">A choice whose bit <see cref="Choices"/> sets.</param>
    /// <returns>The cell the piece lands in.</returns>
    static abstract int CellClaimed(ulong open, int choice);
}
