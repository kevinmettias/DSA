namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixII;

// LeetCode 59. Spiral Matrix II: fill an n x n matrix with 1..n^2 in spiral order.
// The matrix is a plain int[][] - same shape this repo's Spiral Matrix (54)
// coverage already uses - and every cell is written exactly once via
// boundary-shrinking index arithmetic, so there's no repo Representation/
// Operations primitive to compose here; nothing exists that needs composing.
public sealed class SpiralMatrixIITests
{
    [Fact]
    public void GenerateMatrix_ThreeByThree_FillsBoundsClockwise()
    {
        Assert.Equal([[1, 2, 3], [8, 9, 4], [7, 6, 5]], GenerateMatrix(3));
    }

    [Fact]
    public void GenerateMatrix_SingleCell_ReturnsOne()
    {
        Assert.Equal([[1]], GenerateMatrix(1));
    }

    private static int[][] GenerateMatrix(int n)
    {
        var matrix = Enumerable.Range(0, n).Select(_ => new int[n]).ToArray();
        var value = 1;
        var top = 0;
        var bottom = n - 1;
        var left = 0;
        var right = n - 1;

        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++)
            {
                matrix[top][c] = value++;
            }
            top++;

            for (var r = top; r <= bottom; r++)
            {
                matrix[r][right] = value++;
            }
            right--;

            for (var c = right; c >= left && top <= bottom; c--)
            {
                matrix[bottom][c] = value++;
            }
            bottom--;

            for (var r = bottom; r >= top && left <= right; r--)
            {
                matrix[r][left] = value++;
            }
            left++;
        }

        return matrix;
    }
}
