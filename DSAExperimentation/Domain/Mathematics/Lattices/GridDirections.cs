using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Mathematics.Lattices;

// The neighbour offsets a grid move set is made of, each a named GridStep: facts about the square
// lattice, read by DataStructures' grid engines, Domain's chess pieces and the solutions alike (the
// owner's ruling, 2026-11-03). Order is part of the
// contract: Orthogonal is up, down, left, right - the order GridChildren yields neighbours in - and
// the other three are row-major, so an answer that lists cells in the order they were reached comes
// out the same from every caller. GridNeighbors yields in the order of whichever table it is given. A table whose order means something else - a turning cycle for a
// spiral walk or a robot's heading, a puzzle's own numbered direction codes - is not a neighbour set,
// and keeps its own table; a compass heading's is Domain/Devices/Compass's CompassRules.
//
// ImmutableArray rather than a ReadOnlySpan property: a span of tuples cannot be backed by static
// data, so the property would build a new array on every access, and a span cannot be enumerated
// across a yield, which many grid searches do. ImmutableArray cannot be changed by a caller, indexes
// straight into its array, and enumerates with a struct enumerator.
/// <summary>The offset tables of the grid move sets, each a list of named steps in a fixed order.</summary>
public static class GridDirections
{
    public static readonly ImmutableArray<GridStep> Orthogonal =
        [GridStep.Up, GridStep.Down, GridStep.Left, GridStep.Right];

    public static readonly ImmutableArray<GridStep> Diagonal =
        [GridStep.UpLeft, GridStep.UpRight, GridStep.DownLeft, GridStep.DownRight];

    // Every orthogonal and diagonal neighbour: the eight directions of lines on a grid.
    public static readonly ImmutableArray<GridStep> Octilinear =
        [GridStep.UpLeft, GridStep.Up, GridStep.UpRight, GridStep.Left, GridStep.Right, GridStep.DownLeft, GridStep.Down, GridStep.DownRight];

    // The eight L-shaped jumps: two squares along one axis and one along the other.
    public static readonly ImmutableArray<GridStep> LShaped =
        [GridStep.UpUpLeft, GridStep.UpUpRight, GridStep.UpLeftLeft, GridStep.UpRightRight, GridStep.DownLeftLeft, GridStep.DownRightRight, GridStep.DownDownLeft, GridStep.DownDownRight];
}
