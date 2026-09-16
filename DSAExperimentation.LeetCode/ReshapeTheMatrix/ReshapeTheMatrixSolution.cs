namespace DSAExperimentation.LeetCode.ReshapeTheMatrix;

// LeetCode 566. Reshape the Matrix: refill a matrix with targetRows rows and
// targetCols columns from an existing matrix's cells in row-major order, or return
// the original unchanged when the two shapes don't hold the same number of cells.
internal static class ReshapeTheMatrixSolution
{
    // Recomputes both the source and destination row/col from a flat index for
    // every cell.
    public static int[][] ReshapeByLinearIndexDivMod(int[][] mat, int targetRows, int targetCols)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        if (rows * cols != targetRows * targetCols)
        {
            return mat;
        }

        var reshaped = Enumerable.Range(0, targetRows).Select(_ => new int[targetCols]).ToArray();

        for (var i = 0; i < rows * cols; i++)
        {
            reshaped[i / targetCols][i % targetCols] = mat[i / cols][i % cols];
        }

        return reshaped;
    }

    // Walks a destination cursor directly, wrapping only when a row fills up -
    // no division or modulo per cell.
    public static int[][] ReshapeByCursorWalk(int[][] mat, int targetRows, int targetCols)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        if (rows * cols != targetRows * targetCols)
        {
            return mat;
        }

        var reshaped = Enumerable.Range(0, targetRows).Select(_ => new int[targetCols]).ToArray();
        CopyWithCursorWalk(reshaped, mat, cols, targetCols);

        return reshaped;
    }

    // Copies mat's cells across in row-major order, advancing a destination cursor
    // that wraps to the next destination row only when one fills up.
    private static void CopyWithCursorWalk(int[][] reshaped, int[][] mat, int cols, int destCols)
    {
        var destRow = 0;
        var destCol = 0;

        for (var row = 0; row < mat.Length; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                reshaped[destRow][destCol] = mat[row][col];
                destCol++;

                if (destCol != destCols)
                {
                    continue;
                }

                destCol = 0;
                destRow++;
            }
        }
    }
}
