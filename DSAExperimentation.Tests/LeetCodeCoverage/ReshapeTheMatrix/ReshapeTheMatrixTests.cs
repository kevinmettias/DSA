namespace DSAExperimentation.Tests.LeetCodeCoverage.ReshapeTheMatrix;

// LeetCode 566. Reshape the Matrix: the matrix is a plain int[][] (same shape as
// this repo's existing SpiralMatrix/SpiralMatrix II coverage), and every cell
// moves exactly once via row-major linear-index arithmetic - there's no
// Representation/Operations primitive to compose here, nothing exists that needs
// composing.
public sealed class ReshapeTheMatrixTests
{
    [Fact]
    public void MatrixReshape_TwoByTwoToOneByFour_PreservesRowMajorOrder()
    {
        int[][] mat = [[1, 2], [3, 4]];

        Assert.Equal([[1, 2, 3, 4]], MatrixReshape(mat, 1, 4));
    }

    [Fact]
    public void MatrixReshape_IncompatibleDimensions_ReturnsOriginalMatrix()
    {
        int[][] mat = [[1, 2], [3, 4]];

        Assert.Equal([[1, 2], [3, 4]], MatrixReshape(mat, 2, 4));
    }

    private static int[][] MatrixReshape(int[][] mat, int r, int c)
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
}
