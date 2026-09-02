using DiagonalStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DiagonalTraverse;

// LeetCode 498. Diagonal Traverse: group cells by row+col diagonal index and
// collect each diagonal in row-ascending order; every OTHER diagonal must come
// out reversed, so pushing that diagonal's values onto this repo's own LIFO
// Stack<int> and popping them back off - the same digit/row-reversal primitive
// RotateImageTests composes - reverses it with no separate reversal loop.
public sealed partial class DiagonalTraverseTests
{
    [Fact]
    public void FindDiagonalOrder_ThreeByThreeMatrix_ZigZagsAlongDiagonals()
    {
        int[][] matrix = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        Assert.Equal([1, 2, 4, 7, 5, 3, 6, 8, 9], FindDiagonalOrder(matrix));
    }

    [Fact]
    public void FindDiagonalOrder_TwoByTwoMatrix_ZigZagsAlongDiagonals()
    {
        int[][] matrix = [[1, 2], [3, 4]];

        Assert.Equal([1, 2, 3, 4], FindDiagonalOrder(matrix));
    }

    [Fact]
    public void FindDiagonalOrder_SingleRow_ReturnsRowAsIs()
    {
        int[][] matrix = [[1, 2, 3, 4]];

        Assert.Equal([1, 2, 3, 4], FindDiagonalOrder(matrix));
    }

    private static int[] FindDiagonalOrder(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = new int[rows * cols];
        var next = 0;

        for (var diagonal = 0; diagonal <= rows + cols - 2; diagonal++)
        {
            var range = new DiagonalRange(diagonal, Math.Max(0, diagonal - cols + 1), Math.Min(diagonal, rows - 1));

            next = diagonal % 2 == 0
                ? WriteReversedDiagonal(matrix, range, result, next)
                : WriteStraightDiagonal(matrix, range, result, next);
        }

        return result;
    }

    private readonly record struct DiagonalRange(int Diagonal, int RowStart, int RowEnd);

    private static int WriteReversedDiagonal(int[][] matrix, DiagonalRange range, int[] result, int next)
    {
        var reversed = new DiagonalStack();

        for (var r = range.RowStart; r <= range.RowEnd; r++)
        {
            reversed.Push(matrix[r][range.Diagonal - r]);
        }

        for (var r = range.RowStart; r <= range.RowEnd; r++)
        {
            reversed.TryPop(out result[next++]);
        }

        return next;
    }

    private static int WriteStraightDiagonal(int[][] matrix, DiagonalRange range, int[] result, int next)
    {
        for (var r = range.RowStart; r <= range.RowEnd; r++)
        {
            result[next++] = matrix[r][range.Diagonal - r];
        }

        return next;
    }
}
