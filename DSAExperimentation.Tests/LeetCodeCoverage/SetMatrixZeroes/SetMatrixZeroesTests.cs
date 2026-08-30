using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SetMatrixZeroes;

// LeetCode 73. Set Matrix Zeroes: two of this repo's own Set<int> instances
// tracking which rows and which columns contain a zero, then a second pass
// zeroes every cell whose row or column is in either set - the standard
// O(rows+cols)-extra-space approach, composing Set<int> twice.
public sealed class SetMatrixZeroesTests
{
    [Fact]
    public void SetZeroes_OneZeroCell_ZeroesItsRowAndColumn()
    {
        int[][] matrix = [[1, 1, 1], [1, 0, 1], [1, 1, 1]];

        SetZeroes(matrix);

        Assert.Equal([[1, 0, 1], [0, 0, 0], [1, 0, 1]], matrix);
    }

    [Fact]
    public void SetZeroes_ZeroInFirstRowAndColumn_ZeroesBoth()
    {
        int[][] matrix = [[0, 1, 2], [3, 4, 5], [1, 3, 1]];

        SetZeroes(matrix);

        Assert.Equal([[0, 0, 0], [0, 4, 5], [0, 3, 1]], matrix);
    }

    private static void SetZeroes(int[][] matrix)
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
