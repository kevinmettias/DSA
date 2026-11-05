namespace DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

// A board's surface is a rectangle of cells with each pair of opposite edges either left open, as a
// boundary a step cannot cross, or glued, so a step off one edge comes back in at the other. Gluing
// is chosen per pair, and the two choices together fix the surface: none glued is the rectangle
// itself, one pair the cylinder, both pairs the torus; one pair glued the opposite way round makes
// the Mobius strip, and with the other pair glued straight, the Klein bottle. This is one pair's
// rule, along one axis.
//
// A step may be longer than the board is wide (a knight on a two-column torus), so a glued edge is
// crossed as many times as the step needs, and the gluing reports how many. The number is signed:
// crossings toward lower coordinates count negative.
/// <summary>How one pair of a rectangle's opposite edges is joined, along the axis that runs between them.</summary>
public interface IEdgeGluing
{
    /// <summary>Gets whether the edges are joined the opposite way round, so crossing them mirrors the other axis.</summary>
    static abstract bool IsTwisted { get; }

    /// <summary>Brings a coordinate that may lie past an edge back onto the board, if these edges let it.</summary>
    /// <param name="coordinate">The coordinate along this axis; on return, where it lands.</param>
    /// <param name="extent">How many cells the board has along this axis.</param>
    /// <param name="crossings">How many times the coordinate crossed a glued edge, negative toward lower coordinates.</param>
    /// <returns>Whether the coordinate lands on the board.</returns>
    static abstract bool TryWrap(ref int coordinate, int extent, out int crossings);
}
