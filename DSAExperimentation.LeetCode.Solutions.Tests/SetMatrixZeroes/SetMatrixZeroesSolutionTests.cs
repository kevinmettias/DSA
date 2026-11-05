using DSAExperimentation.LeetCode.Solutions.SetMatrixZeroes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SetMatrixZeroes;

// Harness only. Both strategies are SetMatrixZeroesSolution's - this file pins
// them to a single zero in the top-left corner, which clears the whole first row
// and first column. Each row is cloned before zeroing so the two theory methods
// never share a mutated matrix. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class SetMatrixZeroesSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            { [[0, 1, 2], [3, 4, 5], [1, 3, 1]], [[0, 0, 0], [0, 4, 5], [0, 3, 1]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SetZeroesByCopyAndScan_Cases_ZeroesRowsAndColumnsInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        SetMatrixZeroesSolution.SetZeroesByCopyAndScan(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SetZeroesByRowColumnSets_Cases_ZeroesRowsAndColumnsInPlace(
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
