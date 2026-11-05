namespace DSAExperimentation.Domain.Games.Sudoku.Regions;

// A board n = BoxRows x BoxColumns a side is cut into bands of BoxRows rows and stacks of
// BoxColumns columns. A band holds n / BoxColumns boxes side by side, which is BoxRows of them, so
// the boxes are numbered row by row as band x BoxRows + stack. That count is why BoxRows appears
// below where a reader might expect a count of boxes; a 2x3 box shows the two apart, a square box
// does not.
/// <summary>The box arithmetic: a cell's box, and a box's cells and symbols.</summary>
/// <typeparam name="TBox">The grid whose boxes these are.</typeparam>
internal static class BoxPartition<TBox>
    where TBox : struct, IBoxPartition<TBox>
{
    /// <summary>Numbers the box a cell lies in, row by row from 0 at the top left.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The box's number: its band times the boxes in a band, plus its stack.</returns>
    public static int RegionOf(int row, int col) =>
        (row / TBox.BoxRows * TBox.BoxRows) + (col / TBox.BoxColumns);

    /// <summary>Finds one cell of a box, its cells counted row by row.</summary>
    /// <param name="region">The box's number.</param>
    /// <param name="index">Which of the box's cells, from 0.</param>
    /// <returns>The cell's row and column.</returns>
    public static (int Row, int Col) CellOf(int region, int index)
    {
        var top = region / TBox.BoxRows * TBox.BoxRows;
        var left = region % TBox.BoxRows * TBox.BoxColumns;

        return (top + (index / TBox.BoxColumns), left + (index % TBox.BoxColumns));
    }

    /// <summary>Whether the box around a cell already holds a symbol.</summary>
    /// <param name="board">The board, one array of characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="symbol">The symbol to look for.</param>
    /// <returns><see langword="true"/> when some cell of the box holds the symbol.</returns>
    public static bool IsInRegion(char[][] board, int row, int col, char symbol)
    {
        var top = row / TBox.BoxRows * TBox.BoxRows;
        var left = col / TBox.BoxColumns * TBox.BoxColumns;

        for (var r = top; r < top + TBox.BoxRows; r++)
        {
            for (var c = left; c < left + TBox.BoxColumns; c++)
            {
                if (board[r][c] == symbol)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
