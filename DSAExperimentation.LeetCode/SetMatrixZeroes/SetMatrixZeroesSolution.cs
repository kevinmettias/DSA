using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.SetMatrixZeroes;

// LeetCode 73. Set Matrix Zeroes: for every zero cell, zero its entire row and
// column - in place, without letting a cell zeroed on this pass be mistaken
// for an original zero on a later one.
//
// The two strategies differ only in how they remember which cells were zero
// to begin with: a full second copy of the matrix, or this repo's own
// Set<int>, tracking just the zero rows and columns.
internal static class SetMatrixZeroesSolution
{
    // The textbook baseline: snapshot the whole matrix before mutating it, so
    // zero-detection never reads an already-zeroed cell. O(rows*cols) extra
    // space. Deliberately written without this repo's primitives - it is the
    // arm the composed solution below has to justify itself against.
    public static void SetZeroesByCopyAndScan(int[][] matrix)
    {
        var snapshot = CloneMatrix(matrix);

        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (snapshot[r][c] == 0)
                {
                    ZeroRowAndColumn(matrix, r, c);
                }
            }
        }
    }

    private static int[][] CloneMatrix(int[][] source) =>
        source.Select(row => (int[])row.Clone()).ToArray();

    private static void ZeroRowAndColumn(int[][] matrix, int row, int col)
    {
        for (var c = 0; c < matrix[0].Length; c++)
        {
            matrix[row][c] = 0;
        }

        for (var r = 0; r < matrix.Length; r++)
        {
            matrix[r][col] = 0;
        }
    }

    // Track only the zero rows and zero columns in this repo's own Set<int> -
    // O(rows+cols) extra space instead of a full second matrix.
    public static void SetZeroesByRowColumnSets(int[][] matrix)
    {
        var (zeroRows, zeroCols) = CollectZeroRowsAndColumns(matrix);

        ApplyZeroRowsAndColumns(matrix, zeroRows, zeroCols);
    }

    private static (Set<int> Rows, Set<int> Columns) CollectZeroRowsAndColumns(int[][] matrix)
    {
        var zeroRows = new Set<int>();
        var zeroCols = new Set<int>();

        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (matrix[r][c] == 0)
                {
                    zeroRows.TryAdd(r);
                    zeroCols.TryAdd(c);
                }
            }
        }

        return (zeroRows, zeroCols);
    }

    // Reads the collected rows and columns before writing anything, so a cell
    // zeroed on this pass is never mistaken for an original zero.
    private static void ApplyZeroRowsAndColumns(int[][] matrix, Set<int> zeroRows, Set<int> zeroCols)
    {
        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (zeroRows.Has(r) || zeroCols.Has(c))
                {
                    matrix[r][c] = 0;
                }
            }
        }
    }
}
