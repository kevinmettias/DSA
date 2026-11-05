namespace DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

// Edges glued the opposite way round: a step off one edge comes back in at the other, mirrored, as on
// a Mobius strip or a Klein bottle. Along this axis it wraps as a straight gluing does; the mirror
// falls on the other axis, once for each crossing, which GluedRectangle applies.
/// <summary>A pair of edges glued the opposite way round, so a step off one comes back in at the other, mirrored.</summary>
public readonly struct Twisted : IEdgeGluing
{
    /// <inheritdoc/>
    public static bool IsTwisted => true;

    /// <inheritdoc/>
    public static bool TryWrap(ref int coordinate, int extent, out int crossings) =>
        Straight.TryWrap(ref coordinate, extent, out crossings);
}
