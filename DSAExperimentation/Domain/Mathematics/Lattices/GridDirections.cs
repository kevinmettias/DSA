using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Mathematics.Lattices;

// The square lattice's step tables, each a list of named GridSteps: facts about the grid, read by
// DataStructures' grid engines, Domain's chess pieces and the solutions alike (the owner's ruling,
// 2026-11-03). Each neighbour table agrees with the lattice: its steps are a union of orbits under
// the square's symmetries (LatticeSymmetry), which the tests check.
//
// Order is part of the contract:
// - Orthogonal is up, down, left, right, the order GridChildren yields neighbours in, so the
//   opposite of the step at index i is the one at i ^ 1.
// - Diagonal, Octilinear, LShaped and the two distance neighbourhoods are row-major, so an answer
//   that lists cells in the order they were reached comes out the same from every caller.
// - Clockwise is a turning cycle, not a neighbour set: up, right, down, left, numbered as
//   CompassHeading is, so a spiral walk or a robot turns by moving one index along it.
// A puzzle's own numbered direction codes keep their own order, built from the named steps.
//
// ImmutableArray rather than a ReadOnlySpan property: a span of tuples cannot be backed by static
// data, so the property would build a new array on every access, and a span cannot be enumerated
// across a yield, which many grid searches do. ImmutableArray cannot be changed by a caller, indexes
// straight into its array, and enumerates with a struct enumerator.
/// <summary>The square lattice's step tables, each a list of named steps in a fixed order, and the index rules for turning along them.</summary>
public static class GridDirections
{
    // Clockwise holds a whole turn of quarter turns, a power of two, so a turn wraps with a mask.
    private const int ClockwiseMask = GridStep.QuarterTurnsPerTurn - 1;

    // Orthogonal pairs each step with its opposite at the neighbouring even and odd index.
    private const int OppositePairBit = 1;

    /// <summary>The four steps to a cell's edge-sharing neighbours: up, down, left, right.</summary>
    public static readonly ImmutableArray<GridStep> Orthogonal =
        [GridStep.Up, GridStep.Down, GridStep.Left, GridStep.Right];

    /// <summary>The four steps to a cell's corner-sharing neighbours, row by row.</summary>
    public static readonly ImmutableArray<GridStep> Diagonal =
        [GridStep.UpLeft, GridStep.UpRight, GridStep.DownLeft, GridStep.DownRight];

    // Every orthogonal and diagonal neighbour: the eight directions of lines on a grid.
    /// <summary>The eight steps to every neighbour that shares an edge or a corner, row by row.</summary>
    public static readonly ImmutableArray<GridStep> Octilinear =
        [GridStep.UpLeft, GridStep.Up, GridStep.UpRight, GridStep.Left, GridStep.Right, GridStep.DownLeft, GridStep.Down, GridStep.DownRight];

    // The eight L-shaped jumps: two squares along one axis and one along the other.
    /// <summary>The eight L-shaped jumps, two cells along one axis and one along the other, row by row.</summary>
    public static readonly ImmutableArray<GridStep> LShaped =
        [GridStep.UpUpLeft, GridStep.UpUpRight, GridStep.UpLeftLeft, GridStep.UpRightRight, GridStep.DownLeftLeft, GridStep.DownRightRight, GridStep.DownDownLeft, GridStep.DownDownRight];

    /// <summary>The four orthogonal steps in turning order, each a quarter turn clockwise of the one before: up, right, down, left.</summary>
    public static readonly ImmutableArray<GridStep> Clockwise =
        [GridStep.Up, GridStep.Right, GridStep.Down, GridStep.Left];

    /// <summary>The index in <see cref="Clockwise"/> of the step a quarter turn clockwise of the one at the given index.</summary>
    /// <param name="clockwiseIndex">An index into <see cref="Clockwise"/>.</param>
    /// <returns>The index after turning right.</returns>
    public static int TurnRight(int clockwiseIndex) => (clockwiseIndex + 1) & ClockwiseMask;

    /// <summary>The index in <see cref="Clockwise"/> of the step a quarter turn anticlockwise of the one at the given index.</summary>
    /// <param name="clockwiseIndex">An index into <see cref="Clockwise"/>.</param>
    /// <returns>The index after turning left.</returns>
    public static int TurnLeft(int clockwiseIndex) => (clockwiseIndex + ClockwiseMask) & ClockwiseMask;

    /// <summary>The index in <see cref="Orthogonal"/> of the step opposite the one at the given index: Up's is Down's, Left's is Right's.</summary>
    /// <param name="orthogonalIndex">An index into <see cref="Orthogonal"/>.</param>
    /// <returns>The index of the opposite step.</returns>
    public static int OppositeOrthogonal(int orthogonalIndex) => orthogonalIndex ^ OppositePairBit;

    /// <summary>
    /// Every step within a Manhattan distance of the cell, the cell itself left out, row by row: the von Neumann neighbourhood of that range.
    /// </summary>
    /// <param name="range">The most rows and columns together a step may cross.</param>
    /// <returns>The steps, 2 * range * (range + 1) of them.</returns>
    public static ImmutableArray<GridStep> WithinManhattan(int range) => Within<ManhattanReach>(range);

    /// <summary>
    /// Every step within a Chebyshev distance of the cell, the cell itself left out, row by row: the Moore neighbourhood of that range.
    /// </summary>
    /// <param name="range">The most rows, and the most columns, a step may cross.</param>
    /// <returns>The steps, (2 * range + 1)² - 1 of them.</returns>
    public static ImmutableArray<GridStep> WithinChebyshev(int range) => Within<ChebyshevReach>(range);

    private static ImmutableArray<GridStep> Within<TReach>(int range)
        where TReach : struct, IReach
    {
        var steps = ImmutableArray.CreateBuilder<GridStep>();

        for (var deltaRow = -range; deltaRow <= range; deltaRow++)
        {
            var reach = TReach.ColumnsReached(range, deltaRow);

            for (var deltaCol = -reach; deltaCol <= reach; deltaCol++)
            {
                var step = new GridStep(deltaRow, deltaCol);
                var isMove = step != GridStep.AdditiveIdentity;

                if (isMove)
                {
                    steps.Add(step);
                }
            }
        }

        return steps.ToImmutable();
    }

    // How many columns either way a distance reaches on a row some rows from the cell.
    private interface IReach
    {
        static abstract int ColumnsReached(int range, int deltaRow);
    }

    // Rows and columns crossed together: what a row uses up, the columns cannot.
    private readonly struct ManhattanReach : IReach
    {
        public static int ColumnsReached(int range, int deltaRow) => range - Math.Abs(deltaRow);
    }

    // Rows and columns each up to the range: every row reaches as far.
    private readonly struct ChebyshevReach : IReach
    {
        public static int ColumnsReached(int range, int deltaRow) => range;
    }
}


// TODO: CAN THIS GENERALIZE TO OTHER DISTANCE METRICS BEYOND MANHATTAN AND CHEBYSHEV?

// TODO: CONSIDER ADDING SUPPORT FOR OTHER DISTANCE METRICS LIKE EUCLIDEAN OR CUSTOM WEIGHTED DISTANCES.

// TODO: CAN THIS GENERALIZE TO OTHER LATTICE STRUCTURES, TOPOLOGIES OR HIGHER DIMENSIONS?