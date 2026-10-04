using System.Collections.Immutable;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// A cell's neighbours on a board, as DepthFirstSearch.Traverse's successor function wants them: each
// offset in the direction table applied to the cell, kept when the board has that cell and the filter
// lets the search in, in the table's own order. GridChildren does the same over Grid's fixed
// passability map for the topology engines; this is the form a successor lambda can call, for the
// searches whose "open" is decided by the problem's cells as the search runs.
//
// An iterator, so one object per call - the count every hand-written successor iterator it replaces
// already paid. A loop that allocates nothing today should keep its loop and use GridSize.HasCell.
// A struct enumerable was the alternative, but Traverse takes IEnumerable<T> and reverses it, which
// would box both the struct and its enumerator.
internal static class GridNeighbors
{
    public static IEnumerable<(int Row, int Col)> Of<TFilter>(
        (int Row, int Col) cell,
        GridSize size,
        ImmutableArray<(int DeltaRow, int DeltaCol)> directions,
        TFilter filter)
        where TFilter : struct, IGridCellFilter
    {
        for (var i = 0; i < directions.Length; i++)
        {
            var row = cell.Row + directions[i].DeltaRow;
            var col = cell.Col + directions[i].DeltaCol;

            if (size.HasCell(row, col) && filter.CanEnter(row, col))
            {
                yield return (row, col);
            }
        }
    }
}
