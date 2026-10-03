using System.Collections.Immutable;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// The neighbour offsets a grid move set is made of, as (DeltaRow, DeltaCol). Order is part of the
// contract: Orthogonal is up, down, left, right - the order GridChildren yields neighbours in - and
// the other three are row-major, so an answer that lists cells in the order they were reached comes
// out the same from every caller. A table whose order means something else - a turning cycle for a
// spiral walk or a robot's heading, a puzzle's own numbered direction codes - is not a neighbour set,
// and keeps its own table.
//
// ImmutableArray rather than a ReadOnlySpan property: a span of tuples cannot be backed by static
// data, so the property would build a new array on every access, and a span cannot be enumerated
// across a yield, which many grid searches do. ImmutableArray cannot be changed by a caller, indexes
// straight into its array, and enumerates with a struct enumerator.
internal static class GridDirections
{
    public static readonly ImmutableArray<(int DeltaRow, int DeltaCol)> Orthogonal =
        [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public static readonly ImmutableArray<(int DeltaRow, int DeltaCol)> Diagonal =
        [(-1, -1), (-1, 1), (1, -1), (1, 1)];

    // A chess king's moves: every orthogonal and diagonal neighbour.
    public static readonly ImmutableArray<(int DeltaRow, int DeltaCol)> King =
        [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];

    public static readonly ImmutableArray<(int DeltaRow, int DeltaCol)> Knight =
        [(-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1)];
}
