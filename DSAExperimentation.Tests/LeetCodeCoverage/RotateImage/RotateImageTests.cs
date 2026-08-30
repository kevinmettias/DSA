using RowStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateImage;

// LeetCode 48. Rotate Image: transpose the matrix in place, then reverse each row
// with this repo's own LIFO Stack<T> - the same digit-reversal primitive
// ReverseInteger composes, applied here to a matrix row instead of a decimal digit
// run.
public sealed partial class RotateImageTests
{
    [Fact]
    public void Rotate_ThreeByThreeMatrix_RotatesClockwise()
    {
        int[][] matrix = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        Rotate(matrix);

        Assert.Equal([[7, 4, 1], [8, 5, 2], [9, 6, 3]], matrix);
    }

    [Fact]
    public void Rotate_FourByFourMatrix_RotatesClockwise()
    {
        int[][] matrix = [[5, 1, 9, 11], [2, 4, 8, 10], [13, 3, 6, 7], [15, 14, 12, 16]];

        Rotate(matrix);

        Assert.Equal([[15, 13, 2, 5], [14, 3, 4, 1], [12, 6, 8, 9], [16, 7, 10, 11]], matrix);
    }

    private static void Rotate(int[][] matrix)
    {
        Transpose(matrix);

        foreach (var row in matrix)
        {
            ReverseWithStack(row);
        }
    }

    private static void Transpose(int[][] matrix)
    {
        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = r + 1; c < matrix.Length; c++)
            {
                (matrix[r][c], matrix[c][r]) = (matrix[c][r], matrix[r][c]);
            }
        }
    }

    private static void ReverseWithStack(int[] row)
    {
        var pushed = new RowStack();

        foreach (var value in row)
        {
            pushed.Push(value);
        }

        for (var i = 0; i < row.Length; i++)
        {
            pushed.TryPop(out row[i]);
        }
    }
}
