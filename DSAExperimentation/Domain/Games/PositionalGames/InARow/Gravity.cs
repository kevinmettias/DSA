namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Connect Four's placement: a move names a column, and the piece dropped into it falls down the
// rows until the cell below is a piece, a hole or the floor. So on a board without holes it lands
// on the lowest open cell, and a column whose top cell is taken is full. Cells are numbered row by
// row from the top (MnkGame), so a column's top cell has the column's number and the cell below
// any cell is a whole row, Columns cells, further on. A cell beneath a hole can never be reached,
// so a board with holes under gravity makes those cells holes too, or it can never fill.
/// <summary>
/// Gravity: a move names a column, and the piece falls to the lowest open cell it can reach.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows.</typeparam>
public readonly struct Gravity<TShape> : IPlacementRule
    where TShape : struct, IMnkShape
{
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
}
