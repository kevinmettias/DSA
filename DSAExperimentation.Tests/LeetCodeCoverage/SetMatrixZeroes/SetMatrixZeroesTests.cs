using DSAExperimentation.LeetCode.SetMatrixZeroes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SetMatrixZeroes;

// Harness only. Both strategies are SetMatrixZeroesSolution's - this file just
// pins them to LeetCode's published examples. Each row is cloned before
// zeroing so the two theory methods never share a mutated matrix.
public sealed class SetMatrixZeroesTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            { [[1, 1, 1], [1, 0, 1], [1, 1, 1]], [[1, 0, 1], [0, 0, 0], [1, 0, 1]] },
            { [[0, 1, 2], [3, 4, 5], [1, 3, 1]], [[0, 0, 0], [0, 4, 5], [0, 3, 1]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SetZeroesByCopyAndScan_LeetCodeExamples_ZeroesRowsAndColumnsInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        SetMatrixZeroesSolution.SetZeroesByCopyAndScan(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SetZeroesByRowColumnSets_LeetCodeExamples_ZeroesRowsAndColumnsInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        SetMatrixZeroesSolution.SetZeroesByRowColumnSets(working);

        Assert.Equal(expected, working);
    }

    private static int[][] Clone(int[][] matrix)
    {
        var copy = new int[matrix.Length][];

        for (var r = 0; r < matrix.Length; r++)
        {
            copy[r] = (int[])matrix[r].Clone();
        }

        return copy;
    }
}
