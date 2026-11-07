namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

// How often a tiling may use each of its pieces, as the columns its exact cover matrix gives the
// pieces. A piece used exactly once is a primary column, met by exactly one of its placements, as
// in the pentomino puzzles that use each piece once; a piece used at most once is a secondary
// column, met by one placement or none (Knuth's generalization); and a piece any number of times
// has no column, so its placements meet only the region's cells, as dominoes tiling a board do. A
// placement meets its piece's column, when there is one, right after the region's cells' columns.
/// <summary>How often a tiling may use each piece: the piece columns of its exact cover.</summary>
public interface IPieceSupply
{
    /// <summary>Counts the primary columns the pieces add, each met exactly once.</summary>
    /// <param name="pieces">How many pieces there are.</param>
    /// <returns>One per piece, or none.</returns>
    static abstract int PrimaryColumnsFor(int pieces);

    /// <summary>Counts the secondary columns the pieces add, each met at most once.</summary>
    /// <param name="pieces">How many pieces there are.</param>
    /// <returns>One per piece, or none.</returns>
    static abstract int SecondaryColumnsFor(int pieces);
}
