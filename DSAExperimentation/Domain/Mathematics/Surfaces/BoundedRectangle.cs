using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// The board LeetCode's grids are: no edge glued, every edge a boundary.
/// <summary>A board with every edge a boundary, as LeetCode's grids are: a disk, Euler characteristic 1.</summary>
public readonly struct BoundedRectangle : ISurface
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored) =>
        GluedRectangle<Unglued, Unglued>.TryLand(ref row, ref col, board, out mirrored);
}
