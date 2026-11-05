namespace DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

// Edges left open: a boundary. A coordinate lands only when it is already on the board, which one
// unsigned comparison asks, since a negative coordinate reads as a very large unsigned one.
/// <summary>A pair of edges left open, as a boundary no step crosses.</summary>
public readonly struct Unglued : IEdgeGluing
{
    /// <inheritdoc/>
    public static bool IsTwisted => false;

    /// <inheritdoc/>
    public static bool TryWrap(ref int coordinate, int extent, out int crossings)
    {
        crossings = 0;

        return (uint)coordinate < (uint)extent;
    }
}
