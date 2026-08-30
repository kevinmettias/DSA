namespace DSAExperimentation.Tests.LeetCodeCoverage.TransposeMatrix;

// LeetCode 867. Transpose Matrix: flip an m x n matrix over its main diagonal into
// a new n x m matrix. Pure double-index array traversal - the same "no repo
// Representation/Operations primitive to compose" shape SpiralMatrixII/SpiralMatrix
// already establish for fixed-shape grid index arithmetic; Grid/GridChildren model
// unordered orthogonal adjacency for graph walks, not a diagonal-flip copy, so
// forcing this through Grid/** would not be a genuine fit.
public sealed class TransposeMatrixTests
{
    [Fact]
    public void Transpose_SquareMatrix_FlipsOverMainDiagonal()
    {
        int[][] matrix = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        var result = Transpose(matrix);

        Assert.Equal([[1, 4, 7], [2, 5, 8], [3, 6, 9]], result);
    }

    [Fact]
    public void Transpose_RectangularMatrix_SwapsRowAndColumnCounts()
    {
        int[][] matrix = [[1, 2, 3], [4, 5, 6]];

        var result = Transpose(matrix);

        Assert.Equal([[1, 4], [2, 5], [3, 6]], result);
    }

    private static int[][] Transpose(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = new int[cols][];

        for (var c = 0; c < cols; c++)
        {
            result[c] = new int[rows];
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                result[c][r] = matrix[r][c];
            }
        }

        return result;
    }
}
