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
            var rowStart = Math.Max(0, diagonal - cols + 1);
            var rowEnd = Math.Min(diagonal, rows - 1);

            if (diagonal % 2 == 0)
            {
                var reversed = new DiagonalStack();

                for (var r = rowStart; r <= rowEnd; r++)
                {
                    reversed.Push(matrix[r][diagonal - r]);
                }

                for (var r = rowStart; r <= rowEnd; r++)
                {
                    reversed.TryPop(out result[next++]);
                }
            }
            else
            {
                for (var r = rowStart; r <= rowEnd; r++)
                {
                    result[next++] = matrix[r][diagonal - r];
                }
            }
        }

        return result;
    }
}
