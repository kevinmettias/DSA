using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// A rectangle with each pair of opposite edges glued its own way: TRows says what the top and bottom
// edges do, along the rows, and TCols what the left and right edges do, along the columns. The
// presets name the common ones.
//
// Crossing a twisted pair an odd number of times mirrors the other axis: off the right edge of a
// Mobius strip, row r comes back in as row rows - 1 - r. At most one pair may be twisted. Two make
// the projective plane, which a flat grid cannot be (ISurface), and a diagonal step out of a corner
// would come back to its own cell. With one, the mirror and the other axis's wrap commute, both being
// arithmetic modulo the extent, so the order they are applied in cannot change where a step lands.
//
// A step that falls off an edge left open is refused at once, before the other axis is looked at,
// and TryLand returns only constants. That is the bounds check every grid search runs
// (GridSize.HasCell is the bounded rectangle's), and in this shape the JIT compiles it to one
// compare and branch per axis. Evaluating both axes and combining them made a branch-free chain
// that each probe waits on, and LC 1254's search read 3-17% slower; returning the second axis's
// answer as a value left a bool that every caller set and then tested again.
/// <summary>A rectangle whose top and bottom edges are joined by one gluing and whose sides by another.</summary>
/// <typeparam name="TRows">How the top and bottom edges are joined.</typeparam>
/// <typeparam name="TCols">How the left and right edges are joined.</typeparam>
public readonly struct GluedRectangle<TRows, TCols> : ISurface
    where TRows : struct, IEdgeGluing
    where TCols : struct, IEdgeGluing
{
    /// <inheritdoc/>
    public static bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored)
    {
        mirrored = MirroredAxes.None;

        if (!TryCross<TCols>(ref col, ref row, (board.Cols, board.Rows, MirroredAxes.Rows), ref mirrored))
        {
            return false;
        }

        // Refused here rather than returned as a value; the type's comment says why.
        if (!TryCross<TRows>(ref row, ref col, (board.Rows, board.Cols, MirroredAxes.Columns), ref mirrored))
        {
            return false;
        }

        return true;
    }

    // One pair of edges: the coordinate along it wraps, or the step is refused. An odd number of
    // crossings of a twisted pair turns the walker over, so the coordinate across it is mirrored and
    // that axis is added to the ones the landing reports.
    private static bool TryCross<TGluing>(
        ref int along, ref int across, (int Along, int Across, MirroredAxes AcrossAxis) frame, ref MirroredAxes mirrored)
        where TGluing : struct, IEdgeGluing
    {
        if (!TGluing.TryWrap(ref along, frame.Along, out var crossings))
        {
            return false;
        }

        if (TGluing.IsTwisted && IsOdd(crossings))
        {
            across = frame.Across - 1 - across;
            mirrored |= frame.AcrossAxis;
        }

        return true;
    }

    private static bool IsOdd(int crossings) => (crossings & 1) != 0;
}
