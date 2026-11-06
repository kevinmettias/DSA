using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// An m,n,k game: two players take turns claiming cells of a board m columns wide and n rows tall,
// and the first to hold k in a row, along a row, a column or either diagonal, wins. Its cells are
// numbered row by row from the top left, rows growing downward as GridStep's do. Its winning sets
// are every run of k cells along the lattice's line directions (GridDirections.LineDirections)
// that fits on the board, generated once per shape. A step (dr, dc) fits n - (k - 1)|dr| starting
// rows and m - (k - 1)|dc| starting columns, so tic-tac-toe has 3 + 3 + 1 + 1 = 8 lines. A run
// longer than k holds a run of k, so "k or more in a row" wins on the same sets. The three numbers
// are static abstract members of the shape, so a preset's arithmetic folds. A board with holes is
// a BoardMask, and PositionalGame drops the lines through them; other winning conditions are a
// WinningSets of their own.
/// <summary>
/// An m,n,k game's board and its winning lines, every k in a row, generated once per shape.
/// </summary>
/// <typeparam name="TShape">The board's columns and rows, and how many in a row win.</typeparam>
public static class MnkGame<TShape>
    where TShape : struct, IMnkShape
{
    /// <summary>Gets how many cells the board has: its columns times its rows.</summary>
    public static int CellCount => TShape.Columns * TShape.Rows;

    /// <summary>
    /// Gets the winning lines: every run of k cells along a row, a column or a diagonal, in the
    /// line directions' order and row by row within each.
    /// </summary>
    public static WinningSets Lines { get; } = LinesOnTheBoard();

    /// <summary>Gets the whole board, with no holes.</summary>
    public static BoardMask Board { get; } = BoardMask.Full(CellCount);

    /// <summary>Numbers a cell, row by row from the top left.</summary>
    /// <param name="row">The cell's row, from 0 at the top.</param>
    /// <param name="column">The cell's column, from 0 at the left.</param>
    /// <returns>The cell's number.</returns>
    public static int CellAt(int row, int column) => (row * TShape.Columns) + column;

    private static WinningSets LinesOnTheBoard()
    {
        var lines = new List<int[]>();

        foreach (var direction in GridDirections.LineDirections)
        {
            for (var row = 0; row < TShape.Rows; row++)
            {
                for (var column = 0; column < TShape.Columns; column++)
                {
                    AddLineIfItFits(lines, row, column, direction);
                }
            }
        }

        return new WinningSets(CellCount, lines);
    }

    // The run of k cells from a starting cell along a direction, when its last cell is on the
    // board; the board is convex, so then every cell between is too.
    private static void AddLineIfItFits(List<int[]> lines, int row, int column, GridStep direction)
    {
        var lastRow = row + ((TShape.InARow - 1) * direction.DeltaRow);
        var lastColumn = column + ((TShape.InARow - 1) * direction.DeltaCol);
        var fits = (uint)lastRow < (uint)TShape.Rows && (uint)lastColumn < (uint)TShape.Columns;

        if (!fits)
        {
            return;
        }

        var line = new int[TShape.InARow];

        for (var index = 0; index < line.Length; index++)
        {
            var cellRow = row + (index * direction.DeltaRow);
            var cellColumn = column + (index * direction.DeltaCol);
            line[index] = CellAt(cellRow, cellColumn);
        }

        lines.Add(line);
    }
}
