using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// The Mobius strip's sides glued the opposite way round, and its top and bottom glued straight: like
// the torus no boundary and Euler characteristic 0, but a walker that goes once round the rows comes
// back mirrored, which no walk on the torus does.
/// <summary>A board whose top and bottom edges are joined straight and whose sides the opposite way round: a Klein bottle.</summary>
public readonly struct KleinBottle : ISurface
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored) =>
        GluedRectangle<Straight, Twisted>.TryLand(ref row, ref col, board, out mirrored);
}
