using DSAExperimentation.Domain.Mathematics.Metrics;

namespace DSAExperimentation.Domain.Mathematics.Lattices;

// The distance each of GridDirections' neighbour tables makes of the square lattice: the fewest of
// its steps from one cell to another, a graph metric, written as a closed form of the displacement
// between the two cells. On the unbounded lattice:
// - An Orthogonal step covers one axis, one cell, so the distance is the L1 norm (Metrics).
// - An Octilinear step may cover both axes at once, so the distance is the L∞ norm.
// - An LShaped jump makes a metric of its own, which no norm gives, since a norm doubles when the
//   displacement does: one cell along an axis takes 3 jumps, but two cells take 2.
// On a rectangle of cells the first two are unchanged, since a shortest path can keep inside the
// box its two cells span. The L-shaped one changes near the corners: from a corner, the cell
// diagonally inside it takes 4 jumps, not 2. On the boards from 5 by 5 cells the tests try, those
// pairs are the only ones that differ.
/// <summary>The fewest steps of each neighbour table between two lattice cells.</summary>
public static class GridDistances
{
    // An L-shaped jump goes two cells along one axis and one along the other: at most two along the
    // longer axis of a displacement, and three along both together.
    private const int JumpLongLeg = 2;
    private const int JumpShortLeg = 1;
    private const int JumpLength = JumpLongLeg + JumpShortLeg;

    // The two displacements the bounds below undercount, by their longer and shorter axis, and the
    // jumps each takes: one cell along an axis, and two cells along both axes.
    private const int OneAlongAnAxisLonger = 1;
    private const int OneAlongAnAxisShorter = 0;
    private const int OneAlongAnAxisJumps = 3;
    private const int TwoAlongBothAxes = 2;
    private const int TwoAlongBothAxesJumps = 4;

    // The low bit of a count, which is its parity.
    private const int ParityBit = 1;

    /// <summary>The fewest orthogonal steps across a displacement: its L1 length.</summary>
    /// <param name="displacement">The rows and columns from one cell to the other.</param>
    /// <returns>The rows plus the columns crossed.</returns>
    public static int Orthogonal(GridStep displacement) =>
        L1Norm.Length(displacement.DeltaRow, displacement.DeltaCol);

    /// <summary>The fewest octilinear steps across a displacement: its L∞ length.</summary>
    /// <param name="displacement">The rows and columns from one cell to the other.</param>
    /// <returns>The larger of the rows and the columns crossed.</returns>
    public static int Octilinear(GridStep displacement) =>
        LInfinityNorm.Length(displacement.DeltaRow, displacement.DeltaCol);

    // Three bounds, each a fact of one jump: it goes at most two cells along the longer axis, at
    // most three along both together, and it moves an odd number of cells, so it changes the parity
    // of row plus column. The count is the larger of the first two bounds, made one more when its
    // parity is not the displacement's, save the two displacements those bounds undercount.
    /// <summary>The fewest L-shaped jumps across a displacement on the unbounded lattice.</summary>
    /// <param name="displacement">The rows and columns from one cell to the other.</param>
    /// <returns>The fewest jumps, each two cells along one axis and one along the other.</returns>
    public static int LShaped(GridStep displacement)
    {
        var longer = LInfinityNorm.Length(displacement.DeltaRow, displacement.DeltaCol);
        var both = L1Norm.Length(displacement.DeltaRow, displacement.DeltaCol);
        var shorter = both - longer;

        if (longer == OneAlongAnAxisLonger && shorter == OneAlongAnAxisShorter)
        {
            return OneAlongAnAxisJumps;
        }

        if (longer == TwoAlongBothAxes && shorter == TwoAlongBothAxes)
        {
            return TwoAlongBothAxesJumps;
        }

        var alongLonger = DivideRoundingUp(longer, JumpLongLeg);
        var alongBoth = DivideRoundingUp(both, JumpLength);
        var jumps = Math.Max(alongLonger, alongBoth);

        return jumps + ((jumps + both) & ParityBit);
    }

    private static int DivideRoundingUp(int count, int divisor) => (count + divisor - 1) / divisor;
}
