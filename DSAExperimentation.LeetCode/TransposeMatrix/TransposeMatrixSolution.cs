namespace DSAExperimentation.LeetCode.TransposeMatrix;

// LeetCode 867. Transpose Matrix: flip an m x n matrix over its main diagonal into a
// new n x m matrix.
//
// Two strategies, identical in the elements they move and differing only in the
// order they move them:
//
// - IndexSwap is the row-major double loop - the baseline, and what you would write
//   without this repo, so its internals stay BCL.
// - CacheBlocking walks fixed-size sub-blocks so that the row being read and the
//   column being written both stay small enough to sit in cache. A well-known
//   transpose technique, not a repo primitive.
//
// No repo Representation/Operations primitive applies to either shape: Grid and
// GridChildren model unordered orthogonal adjacency for graph walks, not a fixed
// diagonal-flip copy, so routing this through Grid/** would not be a genuine fit -
// the same reasoning SpiralMatrix already states for its index arithmetic.
internal static class TransposeMatrixSolution
{
    private const int BlockSize = 32;

    // Baseline: read the source in row-major order, writing each element to its
    // transposed position.
    public static int[][] TransposeByIndexSwap(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = NewMatrix(cols, rows);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                result[c][r] = matrix[r][c];
            }
        }

        return result;
    }

    // Tile the copy so each block's reads and writes share a small working set.
    public static int[][] TransposeByCacheBlocking(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = NewMatrix(cols, rows);

        for (var blockRow = 0; blockRow < rows; blockRow += BlockSize)
        {
            for (var blockCol = 0; blockCol < cols; blockCol += BlockSize)
            {
                TransposeBlock(matrix, result, blockRow, blockCol);
            }
        }

        return result;
    }

    private static void TransposeBlock(int[][] matrix, int[][] result, int blockRow, int blockCol)
    {
        var rowLimit = Math.Min(blockRow + BlockSize, matrix.Length);
        var colLimit = Math.Min(blockCol + BlockSize, matrix[0].Length);

        for (var r = blockRow; r < rowLimit; r++)
        {
            for (var c = blockCol; c < colLimit; c++)
            {
                result[c][r] = matrix[r][c];
            }
        }
    }

    private static int[][] NewMatrix(int rows, int cols)
    {
        var matrix = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            matrix[r] = new int[cols];
        }

        return matrix;
    }
}
