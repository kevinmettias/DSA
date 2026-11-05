using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// One pair of edges glued the opposite way round, the left and right: off the right edge, row r comes
// back in at the left as row rows - 1 - r. Its two long edges join into one boundary circle.
/// <summary>A board whose left and right edges are joined the opposite way round: a Mobius strip, one boundary circle.</summary>
public readonly struct MobiusStrip : ISurface
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored) =>
        GluedRectangle<Unglued, Twisted>.TryLand(ref row, ref col, board, out mirrored);
}
