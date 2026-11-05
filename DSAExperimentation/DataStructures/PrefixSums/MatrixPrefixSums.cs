using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.PrefixSums;

// PrefixSums over a matrix. TotalBefore(rowEnd, columnEnd) combines the top-left rectangle of values
// above rowEnd and left of columnEnd, so either end at 0 gives Identity, and Query(top, left, bottom,
// right) answers an inclusive rectangle from four of those totals in O(1). LC 304's region sums were
// building this table by hand. LC 1738's coordinate xors build it too, but read every total in a loop,
// where TotalBefore's two bounds checks per cell measured about 10% slower at 300 x 300, so that
// solution keeps its own table.
//
// The four totals are the bottom-right one and the corner above-left of the rectangle, less the strip
// above it and the strip to its left, each of which also holds that corner. Taking a total back out
// is Invert, as in PrefixSums. Regrouping the four is only sound because IAbelianGroup's operations
// commute, which a one-dimensional range never needed to ask.
//
// Rows arrive as a jagged array, the shape the problems hand over. A ragged matrix is refused, because
// a short row has no answer for a rectangle that reaches past its end.
/// <summary>Running totals of a fixed matrix under a group operation, for O(1) rectangle queries.</summary>
public sealed class MatrixPrefixSums<Element, TOperation>
    where TOperation : struct, IAbelianGroup<Element>
{
    private const string RaggedRowsMessage = "Every row must hold as many values as the first.";
    private const string EndOutOfBoundsMessage = "An end was outside [0, RowCount] or [0, ColumnCount] for these prefix sums.";
    private const string RectangleOutOfBoundsMessage = "Rectangle was outside the bounds of these prefix sums, or a side was reversed.";

    // _totals[rowEnd, columnEnd] is the combined value of the first rowEnd rows' first columnEnd values;
    // one row and one column larger than the matrix, both of them Identity.
    private readonly Element[,] _totals;

    /// <summary>The number of rows the totals were built from.</summary>
    public int RowCount => _totals.GetLength(0) - 1;

    /// <summary>The number of values in each row the totals were built from.</summary>
    public int ColumnCount => _totals.GetLength(1) - 1;

    public MatrixPrefixSums(Element[][] rows)
    {
        var columnCount = ColumnCountOf(rows);
        _totals = new Element[rows.Length + 1, columnCount + 1];

        for (var column = 0; column <= columnCount; column++)
        {
            _totals[0, column] = TOperation.Identity;
        }

        for (var row = 0; row < rows.Length; row++)
        {
            if (rows[row].Length != columnCount)
            {
                throw new ArgumentException(RaggedRowsMessage, nameof(rows));
            }

            AddRow(row, rows[row]);
        }
    }

    private static int ColumnCountOf(Element[][] rows)
    {
        if (rows.Length == 0)
        {
            return 0;
        }

        return rows[0].Length;
    }

    // Each total is the one above it combined with this row's running total up to the same column.
    private void AddRow(int row, Element[] values)
    {
        var rowTotal = TOperation.Identity;
        _totals[row + 1, 0] = TOperation.Identity;

        for (var column = 0; column < values.Length; column++)
        {
            rowTotal = TOperation.Combine(rowTotal, values[column]);
            _totals[row + 1, column + 1] = TOperation.Combine(_totals[row, column + 1], rowTotal);
        }
    }

    /// <summary>Combines every value in the top-left rectangle before two exclusive ends.</summary>
    /// <param name="rowEnd">The exclusive row end, 0 through RowCount.</param>
    /// <param name="columnEnd">The exclusive column end, 0 through ColumnCount.</param>
    /// <returns>The rectangle's values combined, <c>TOperation.Identity</c> when either end is 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Either end is outside its [0, count].</exception>
    public Element TotalBefore(int rowEnd, int columnEnd)
    {
        RangeBounds.ValidateIndex(rowEnd, _totals.GetLength(0), EndOutOfBoundsMessage);
        RangeBounds.ValidateIndex(columnEnd, _totals.GetLength(1), EndOutOfBoundsMessage);

        return _totals[rowEnd, columnEnd];
    }

    /// <summary>Combines the values of the inclusive rectangle from (top, left) to (bottom, right) in O(1).</summary>
    /// <param name="top">The 0-based index of the rectangle's first row.</param>
    /// <param name="left">The 0-based index of the rectangle's first column.</param>
    /// <param name="bottom">The 0-based index of the rectangle's last row.</param>
    /// <param name="right">The 0-based index of the rectangle's last column.</param>
    /// <returns>The combined value of the rectangle.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A side is reversed or reaches outside the matrix.</exception>
    public Element Query(int top, int left, int bottom, int right)
    {
        ValidateRectangle(top, left, bottom, right);

        return CombineRectangle(top, left, bottom, right);
    }

    private void ValidateRectangle(int top, int left, int bottom, int right)
    {
        RangeBounds.ValidateRange(top, bottom, RowCount, RectangleOutOfBoundsMessage);
        RangeBounds.ValidateRange(left, right, ColumnCount, RectangleOutOfBoundsMessage);
    }

    private Element CombineRectangle(int top, int left, int bottom, int right)
    {
        var bottomRightAndCorner = TOperation.Combine(_totals[bottom + 1, right + 1], _totals[top, left]);
        var strips = TOperation.Combine(_totals[top, right + 1], _totals[bottom + 1, left]);

        return TOperation.Combine(bottomRightAndCorner, TOperation.Invert(strips));
    }
}
