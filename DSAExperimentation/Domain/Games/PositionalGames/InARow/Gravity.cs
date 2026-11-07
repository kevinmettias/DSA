using System.Numerics;

namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Connect Four's placement: a move names a column, and the piece dropped into it falls down the
// rows until the cell below is a piece, a hole or the floor. So on a board without holes it lands
// on the lowest open cell, and a column whose top cell is taken is full. Cells are numbered row by
// row from the top (MnkGame), so a column's top cell has the column's number and the cell below
// any cell is a whole row, Columns cells, further on. A cell beneath a hole can never be reached,
// so a board with holes under gravity makes those cells holes too, or it can never fill.
//
// On a board held as a bit mask the same rule reads off the column's bits: a column is a choice
// while its top cell is open, so the choices are the open cells of the top row, and the piece
// stops one row above the highest cell of the column that is not open, or on the bottom row when
// every cell of the column is.
/// <summary>
/// Gravity: a move names a column, and the piece falls to the lowest open cell it can reach.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows.</typeparam>
public readonly struct Gravity<TShape> : IPlacementRule, IBitboardPlacement
    where TShape : struct, IMnkShape
{
    // The cells of the leftmost column, one bit a row; column c's are these shifted by c.
    private static readonly ulong FirstColumn = FirstColumnCells();

    /// <inheritdoc/>
    public static bool TryPlace(ReadOnlySpan<int> owners, int move, out int cell)
    {
        cell = move;
        var isColumn = (uint)move < (uint)TShape.Columns;

        if (!isColumn || owners[cell] != BoardMask.Unclaimed)
        {
            return false;
        }

        var below = cell + TShape.Columns;

        while (below < owners.Length && owners[below] == BoardMask.Unclaimed)
        {
            cell = below;
            below += TShape.Columns;
        }

        return true;
    }

    /// <inheritdoc/>
    public static ulong Choices(ulong open) => open & ((1UL << TShape.Columns) - 1);

    /// <inheritdoc/>
    public static int CellClaimed(ulong open, int choice)
    {
        var blocked = ~open & (FirstColumn << choice);

        if (blocked == 0)
        {
            return choice + ((TShape.Rows - 1) * TShape.Columns);
        }

        return BitOperations.TrailingZeroCount(blocked) - TShape.Columns;
    }

    private static ulong FirstColumnCells()
    {
        var cells = 0UL;

        for (var row = 0; row < TShape.Rows; row++)
        {
            cells |= 1UL << (row * TShape.Columns);
        }

        return cells;
    }
}
