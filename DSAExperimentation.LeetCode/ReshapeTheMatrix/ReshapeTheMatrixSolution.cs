namespace DSAExperimentation.LeetCode.ReshapeTheMatrix;

// LeetCode 566. Reshape the Matrix: refill an r x c matrix from an existing
// matrix's cells in row-major order, or return the original unchanged when the
// two shapes don't hold the same number of cells.
internal static class ReshapeTheMatrixSolution
{
    // Recomputes both the source and destination row/col from a flat index for
    // every cell.
    public static int[][] ReshapeByLinearIndexDivMod(int[][] mat, int r, int c)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        if (rows * cols != r * c)
        {
            return mat;
        }

        var reshaped = Enumerable.Range(0, r).Select(_ => new int[c]).ToArray();

        for (var i = 0; i < rows * cols; i++)
        {
            reshaped[i / c][i % c] = mat[i / cols][i % cols];
        }

        return reshaped;
    }

    // Walks a destination cursor directly, wrapping only when a row fills up -
    // no division or modulo per cell.
    public static int[][] ReshapeByCursorWalk(int[][] mat, int r, int c)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        if (rows * cols != r * c)
        {
            return mat;
        }

        var reshaped = Enumerable.Range(0, r).Select(_ => new int[c]).ToArray();
        var destRow = 0;
        var destCol = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                reshaped[destRow][destCol] = mat[row][col];
                destCol++;

                if (destCol != c)
                {
                    continue;
                }

                destCol = 0;
                destRow++;
            }
        }

        return reshaped;
    }
}
