namespace DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

// The Latin square's rule on a square of any order: no symbol twice in a row or a column. A square
// is written one character per cell, as this repo's alphabets write symbols. Which characters are
// the symbols is read off the square itself, its first row, because a Latin square may be written
// in any n symbols; whether they are a puzzle's digits is the puzzle's notation, not the square's.
/// <summary>The Latin square's rule: no symbol appears twice in a row or a column.</summary>
/// <typeparam name="TSquare">The square's order, such as a Sudoku grid.</typeparam>
public static class LatinSquare<TSquare>
    where TSquare : struct, ILatinSquare
{
    /// <summary>Whether a cell's row or column already holds a symbol.</summary>
    /// <param name="square">The square, one array of n characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="symbol">The symbol to look for.</param>
    /// <returns><see langword="true"/> when a cell of the row or the column holds it.</returns>
    public static bool IsInRowOrColumn(char[][] square, int row, int col, char symbol)
    {
        for (var i = 0; i < TSquare.Order; i++)
        {
            if (square[row][i] == symbol || square[i][col] == symbol)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a filled square is a Latin square: n x n, n symbols, none twice in a row or column.
    /// </summary>
    /// <param name="square">The square, one array of characters per row.</param>
    /// <returns>
    /// <see langword="true"/> when the square is n a side and every cell holds one of its first
    /// row's symbols, which no other cell of its row or column holds.
    /// </returns>
    public static bool IsLatinSquare(char[][] square)
    {
        if (!IsOrderSquare(square))
        {
            return false;
        }

        for (var row = 0; row < TSquare.Order; row++)
        {
            for (var col = 0; col < TSquare.Order; col++)
            {
                if (!IsLatinAt(square, row, col))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Whether the square has n rows of n cells each.
    private static bool IsOrderSquare(char[][] square) =>
        square.Length == TSquare.Order &&
        Array.TrueForAll(square, cells => cells.Length == TSquare.Order);

    // Whether a cell holds one of the first row's symbols and no other cell of its row or column
    // holds the same one. Every row then holds n different symbols out of the first row's n, which
    // is each of them once, and so does every column.
    private static bool IsLatinAt(char[][] square, int row, int col)
    {
        var symbol = square[row][col];

        if (Array.IndexOf(square[0], symbol) < 0)
        {
            return false;
        }

        for (var i = 0; i < TSquare.Order; i++)
        {
            if (i != col && square[row][i] == symbol)
            {
                return false;
            }

            if (i != row && square[i][col] == symbol)
            {
                return false;
            }
        }

        return true;
    }
}
