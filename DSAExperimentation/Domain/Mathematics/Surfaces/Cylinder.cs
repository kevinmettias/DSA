using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// One pair of edges glued, the left and right: the rows run round, and the top and bottom stay boundaries.
/// <summary>A board whose left and right edges are joined, so a row runs round: a cylinder, two boundaries.</summary>
public readonly struct Cylinder : ISurface
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored) =>
        GluedRectangle<Unglued, Straight>.TryLand(ref row, ref col, board, out mirrored);
}
