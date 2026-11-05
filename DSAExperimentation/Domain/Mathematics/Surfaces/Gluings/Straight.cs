namespace DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

// Edges glued the same way round: a step off one edge comes back in at the other, as on a cylinder
// or a torus. Every coordinate lands, at its floor remainder by the extent, and the floor quotient
// counts the crossings. A coordinate already on the board, the common case, skips the division.
/// <summary>A pair of edges glued the same way round, so a step off one comes back in at the other.</summary>
public readonly struct Straight : IEdgeGluing
{
    /// <inheritdoc/>
    public static bool IsTwisted => false;

    /// <inheritdoc/>
    public static bool TryWrap(ref int coordinate, int extent, out int crossings)
    {
        var isOnBoard = (uint)coordinate < (uint)extent;

        if (isOnBoard)
        {
            crossings = 0;
            return true;
        }

        crossings = Math.DivRem(coordinate, extent, out var remainder);

        if (remainder < 0)
        {
            crossings--;
            remainder += extent;
        }

        coordinate = remainder;
        return true;
    }
}
