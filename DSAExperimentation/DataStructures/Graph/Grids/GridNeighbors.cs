using System.Collections;
using System.Collections.Immutable;

namespace DSAExperimentation.DataStructures.Graph.Grids;

// A cell's neighbours on a board, as DepthFirstSearch.Traverse's successor function wants them: each
// offset in the direction table applied to the cell, kept when the board has that cell and the filter
// lets the search in, in the table's own order. GridChildren does the same over Grid's fixed
// passability map for the topology engines; this is the form a successor lambda can call, for the
// searches whose "open" is decided by the problem's cells as the search runs.
//
// One object per call - the count every hand-written successor iterator it replaces already paid -
// and a small one. A `yield` iterator returning IEnumerable keeps a second copy of every parameter
// so a later GetEnumerator can start afresh, which made each call's object larger than the
// iterators this replaced; Scan holds each value once and is its own first enumerator. A loop that
// allocates nothing today should keep its loop and use GridSize.HasCell. A struct enumerable was the
// other alternative, but Traverse takes IEnumerable<T> and reverses it, which would box both the
// struct and its enumerator.
internal static class GridNeighbors
{
    public static IEnumerable<(int Row, int Col)> Of<TFilter>(
        (int Row, int Col) cell,
        GridSize size,
        ImmutableArray<(int DeltaRow, int DeltaCol)> directions,
        TFilter filter)
        where TFilter : struct, IGridCellFilter
        => new Scan<TFilter>(cell, size, directions, filter);

    // The scan, written out rather than generated: the enumerable hands itself out as its first
    // enumerator and a fresh copy after that, so enumerating it twice still sees every neighbour.
    private sealed class Scan<TFilter>(
        (int Row, int Col) cell,
        GridSize size,
        ImmutableArray<(int DeltaRow, int DeltaCol)> directions,
        TFilter filter)
        : IEnumerable<(int Row, int Col)>, IEnumerator<(int Row, int Col)>
        where TFilter : struct, IGridCellFilter
    {
        private const int NotYetHandedOut = -1;

        private int _nextDirection = NotYetHandedOut;
        private (int Row, int Col) _current;

        (int Row, int Col) IEnumerator<(int Row, int Col)>.Current => _current;

        object IEnumerator.Current => _current;

        IEnumerator<(int Row, int Col)> IEnumerable<(int Row, int Col)>.GetEnumerator()
        {
            var enumerator = _nextDirection == NotYetHandedOut ? this : new Scan<TFilter>(cell, size, directions, filter);
            enumerator._nextDirection = 0;

            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<(int Row, int Col)>)this).GetEnumerator();

        bool IEnumerator.MoveNext()
        {
            while (_nextDirection < directions.Length)
            {
                var (deltaRow, deltaCol) = directions[_nextDirection++];
                var row = cell.Row + deltaRow;
                var col = cell.Col + deltaCol;

                if (size.HasCell(row, col) && filter.CanEnter(row, col))
                {
                    _current = (row, col);
                    return true;
                }
            }

            return false;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
        }
    }
}
