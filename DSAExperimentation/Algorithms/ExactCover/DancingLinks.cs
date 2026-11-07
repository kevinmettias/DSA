using System.Runtime.CompilerServices;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Algorithms.ExactCover;

// Knuth's dancing links (2000) in the flat layout of his later DLX1 (TAOCP 7.2.2.1): a sparse 0/1
// matrix whose rows and columns can be taken out and put back in constant time per link, because a
// node taken out of a doubly linked list keeps its own links, and putting it back in the reverse
// order restores the list exactly. Every link is an int index into flat arrays, never a node
// object, so a search touches only arrays.
//
// Node 0 is the root of the list of primary columns still to cover, and nodes 1 to C are the
// column headers: the problem's column j is header j + 1, its 1s linked in a vertical circular
// list through Up and Down. A secondary column's header links only to itself sideways, so it is on
// no list the choice of column reads, and is covered only by a chosen row that meets it, which
// takes out every other row meeting it: at most once, as secondary columns ask. After the headers
// each row's nodes lie side by side, with a spacer node before each row and after the last. A
// spacer's Top is minus the number of the row after it (0 or below), a node's Top its column
// (1 or above), so a walk along a row knows a spacer when it meets one. The spacer after a row
// points Up at the row's first node and the spacer before it Down at its last, so a walk can go
// round a row in either direction without left and right links. The constructor makes the arrays
// for the sizes it is given; Of lays a problem out in them.
/// <summary>A sparse 0/1 matrix whose rows and columns are removed and restored in place.</summary>
/// <param name="columnCount">How many columns, primary and secondary.</param>
/// <param name="rowCount">How many rows.</param>
/// <param name="nodeCount">How many nodes: the root, the headers, the spacers and the 1s.</param>
internal sealed class DancingLinks(int columnCount, int rowCount, int nodeCount)
{
    private const int HeaderRoot = 0;

    // The list of primary columns still to cover, through the headers: index 0 is the root.
    private readonly int[] _left = new int[columnCount + 1];
    private readonly int[] _right = new int[columnCount + 1];

    // Each node's neighbours above and below in its column, or for a spacer the nodes that close
    // the rows on either side of it.
    private readonly int[] _up = new int[nodeCount];
    private readonly int[] _down = new int[nodeCount];

    // Each node's column header, or for a spacer minus the number of the row after it.
    private readonly int[] _top = new int[nodeCount];

    // How many rows still in the matrix meet each column, by header.
    private readonly int[] _length = new int[columnCount + 1];

    // Where each row's spacer sits, the row's nodes lying between it and the next row's spacer.
    private readonly int[] _spacers = new int[rowCount + 1];

    /// <summary>Gets whether every primary column is covered: the rows chosen cover.</summary>
    public bool IsComplete => _right[HeaderRoot] == HeaderRoot;

    /// <summary>Lays a problem's rows out as dancing links, every row in the matrix.</summary>
    /// <typeparam name="TProblem">The problem.</typeparam>
    /// <param name="problem">The problem whose rows to lay out.</param>
    /// <returns>The matrix, with no column covered.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A row meets a column the problem does not have.
    /// </exception>
    /// <exception cref="ArgumentException">A row meets one column twice.</exception>
    public static DancingLinks Of<TProblem>(TProblem problem)
        where TProblem : struct, IExactCoverProblem
    {
        var primary = problem.PrimaryColumnCount;
        var columns = primary + problem.SecondaryColumnCount;
        var entries = 0;

        for (var row = 0; row < problem.RowCount; row++)
        {
            entries += problem.ColumnCountOf(row);
        }

        var nodes = columns + 1 + problem.RowCount + 1 + entries;
        var links = new DancingLinks(columns, problem.RowCount, nodes);
        links.LinkHeaders(primary, columns);
        links.LinkRows(problem, columns);

        return links;
    }

    // The primary columns' headers in one circular list from the root, in order, and each
    // secondary column's header on its own; every column's vertical list empty.
    private void LinkHeaders(int primary, int columns)
    {
        for (var column = 1; column <= primary; column++)
        {
            _left[column] = column - 1;
            _right[column - 1] = column;
        }

        _right[primary] = HeaderRoot;
        _left[HeaderRoot] = primary;

        for (var column = primary + 1; column <= columns; column++)
        {
            _left[column] = column;
            _right[column] = column;
        }

        for (var column = 1; column <= columns; column++)
        {
            _up[column] = column;
            _down[column] = column;
        }
    }

    // Every row laid out after the headers, each after its spacer, and a spacer after the last.
    private void LinkRows<TProblem>(TProblem problem, int columns)
        where TProblem : struct, IExactCoverProblem
    {
        var spacer = columns + 1;

        for (var row = 0; row < problem.RowCount; row++)
        {
            _spacers[row] = spacer;
            spacer = LinkRow(problem, row, spacer);
        }

        _spacers[problem.RowCount] = spacer;
    }

    // One row's nodes written after its spacer, each appended to the foot of its column, and the
    // two spacers pointed at the row's ends; the spacer after the row is returned.
    private int LinkRow<TProblem>(TProblem problem, int row, int spacer)
        where TProblem : struct, IExactCoverProblem
    {
        var first = spacer + 1;
        var count = problem.ColumnCountOf(row);
        var columns = _top.AsSpan(first, count);
        problem.WriteColumns(row, columns);

        for (var node = first; node < first + count; node++)
        {
            AppendToColumn(problem, row, node);
        }

        var next = first + count;
        _down[spacer] = next - 1;
        _top[next] = -(row + 1);
        _up[next] = first;

        return next;
    }

    // A node, holding the problem's number for its column, made a 1 of that column: renumbered to
    // the column's header and linked in below the column's last node.
    private void AppendToColumn<TProblem>(TProblem problem, int row, int node)
        where TProblem : struct, IExactCoverProblem
    {
        var column = CheckedHeader(problem, row, _top[node]);
        var last = _up[column];

        _top[node] = column;
        _up[node] = last;
        _down[node] = column;
        _down[last] = node;
        _up[column] = node;
        _length[column]++;
    }

    // The header of the column a row names, refusing a column out of range and one the row named
    // already. A column's last node is its highest, so a last node past the row's spacer is one of
    // the row's own.
    private int CheckedHeader<TProblem>(TProblem problem, int row, int problemColumn)
        where TProblem : struct, IExactCoverProblem
    {
        if ((uint)problemColumn >= (uint)(_length.Length - 1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(problem), $"Row {row} meets column {problemColumn}, out of range.");
        }

        var column = problemColumn + 1;

        if (_up[column] > _spacers[row])
        {
            throw new ArgumentException(
                $"Row {row} meets column {problemColumn} twice.", nameof(problem));
        }

        return column;
    }

    /// <summary>Finds the primary column still to cover that the fewest rows meet.</summary>
    /// <returns>The column's header; of columns met as rarely, the first.</returns>
    public int ColumnWithFewestRows()
    {
        var right = _right;
        var length = _length;
        var best = right[HeaderRoot];

        for (var column = right[best]; column != HeaderRoot; column = right[column])
        {
            if (length[column] < length[best])
            {
                best = column;
            }
        }

        return best;
    }

    /// <summary>Finds the node below another in its column, or the first row of a column.</summary>
    /// <param name="node">A node, or a column's header.</param>
    /// <returns>The node below, which is the header again after the last row.</returns>
    public int Below(int node) => _down[node];

    /// <summary>Finds the row a node lies in.</summary>
    /// <param name="node">A node of a row, not a header or a spacer.</param>
    /// <returns>The row's number.</returns>
    public int RowOf(int node)
    {
        var top = _top;

        while (top[node] > 0)
        {
            node--;
        }

        return -top[node];
    }

    /// <summary>
    /// Covers a column: takes it off the list to cover and takes out every row meeting it.
    /// </summary>
    /// <param name="column">The column's header.</param>
    public void Cover(int column)
    {
        for (var row = _down[column]; row != column; row = _down[row])
        {
            Hide(row);
        }

        _right[_left[column]] = _right[column];
        _left[_right[column]] = _left[column];
    }

    // Takes a row out of every column it meets but the one being covered, walking it left to
    // right from the node in that column. Inlined into Cover, as Knuth writes hide inside cover:
    // as a call it would stand inside Cover's loop, where the JIT keeps no array in a register
    // across it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Hide(int row)
    {
        var up = _up;
        var down = _down;
        var top = _top;
        var length = _length;

        for (var node = row + 1; node != row;)
        {
            var column = top[node];

            if (column <= 0)
            {
                node = up[node];
                continue;
            }

            down[up[node]] = down[node];
            up[down[node]] = up[node];
            length[column]--;
            node++;
        }
    }

    /// <summary>Uncovers a column, undoing <see cref="Cover"/> in the reverse order.</summary>
    /// <param name="column">The header of the column covered last and still covered.</param>
    public void Uncover(int column)
    {
        _right[_left[column]] = column;
        _left[_right[column]] = column;

        for (var row = _up[column]; row != column; row = _up[row])
        {
            Unhide(row);
        }
    }

    // Puts a hidden row back, right to left, the reverse of Hide; inlined into Uncover for the
    // same reason.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Unhide(int row)
    {
        var up = _up;
        var down = _down;
        var top = _top;
        var length = _length;

        for (var node = row - 1; node != row;)
        {
            var column = top[node];

            if (column <= 0)
            {
                node = down[node];
                continue;
            }

            down[up[node]] = node;
            up[down[node]] = node;
            length[column]++;
            node--;
        }
    }

    /// <summary>Covers every other column a node's row meets, left to right.</summary>
    /// <param name="node">A node of the row chosen, whose own column is covered already.</param>
    public void CoverRestOfRow(int node)
    {
        for (var other = node + 1; other != node;)
        {
            var column = _top[other];

            if (column <= 0)
            {
                other = _up[other];
                continue;
            }

            Cover(column);
            other++;
        }
    }

    /// <summary>Uncovers every other column a node's row meets, right to left.</summary>
    /// <param name="node">The node <see cref="CoverRestOfRow"/> was given.</param>
    public void UncoverRestOfRow(int node)
    {
        for (var other = node - 1; other != node;)
        {
            var column = _top[other];

            if (column <= 0)
            {
                other = _down[other];
                continue;
            }

            Uncover(column);
            other--;
        }
    }

    /// <summary>Chooses rows before any search, covering every column each one meets.</summary>
    /// <param name="rows">The rows to choose, each from 0 below the problem's row count.</param>
    /// <returns>
    /// <see langword="true"/> when no two of the rows meet one column; otherwise
    /// <see langword="false"/>, with the matrix part chosen and of no further use.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">A row is not one of the problem's.</exception>
    public bool TryChooseAll(ReadOnlySpan<int> rows)
    {
        var covered = new bool[_length.Length];

        foreach (var row in rows)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(row, nameof(rows));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                row, _spacers.Length - 1, nameof(rows));

            if (!TryChoose(row, covered))
            {
                return false;
            }
        }

        return true;
    }

    // Chooses one row, unless a row chosen before it met one of its columns.
    private bool TryChoose(int row, bool[] covered)
    {
        if (HasCoveredColumn(row, covered))
        {
            return false;
        }

        CoverEachColumnOf(row, covered);

        return true;
    }

    // Whether a row meets a column already covered.
    private bool HasCoveredColumn(int row, bool[] covered)
    {
        for (var node = _spacers[row] + 1; node < _spacers[row + 1]; node++)
        {
            if (covered[_top[node]])
            {
                return true;
            }
        }

        return false;
    }

    // Covers each column a row meets, and marks it covered.
    private void CoverEachColumnOf(int row, bool[] covered)
    {
        for (var node = _spacers[row] + 1; node < _spacers[row + 1]; node++)
        {
            covered[_top[node]] = true;
            Cover(_top[node]);
        }
    }
}
