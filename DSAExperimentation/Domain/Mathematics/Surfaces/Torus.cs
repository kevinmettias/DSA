using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// Both pairs of edges glued: rows and columns run round, and nothing is a boundary.
/// <summary>A board whose opposite edges are both joined, so rows and columns run round: a torus, no boundary.</summary>
public readonly struct Torus : ISurface
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored) =>
        GluedRectangle<Straight, Straight>.TryLand(ref row, ref col, board, out mirrored);
}
