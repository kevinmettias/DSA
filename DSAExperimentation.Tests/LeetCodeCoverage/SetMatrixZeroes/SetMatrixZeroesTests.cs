namespace DSAExperimentation.Tests.LeetCodeCoverage.SetMatrixZeroes;

public sealed class SetMatrixZeroesTests
{
    [Fact]
    public void SetZeroes_UsesFirstRowAndColumnMarkers_ZeroesRowsAndColumns()
    {
        int[][] matrix = [[1,1,1],[1,0,1],[1,1,1]];
        SetZeroes(matrix);
        Assert.Equal([[1,0,1],[0,0,0],[1,0,1]], matrix);
    }

    private static void SetZeroes(int[][] matrix)
    {
        var firstRow = matrix[0].Any(v => v == 0); var firstCol = matrix.Any(row => row[0] == 0);
        for (var r = 1; r < matrix.Length; r++) for (var c = 1; c < matrix[0].Length; c++) if (matrix[r][c] == 0) { matrix[r][0] = 0; matrix[0][c] = 0; }
        for (var r = 1; r < matrix.Length; r++) for (var c = 1; c < matrix[0].Length; c++) if (matrix[r][0] == 0 || matrix[0][c] == 0) matrix[r][c] = 0;
        if (firstRow) Array.Fill(matrix[0], 0);
        if (firstCol) for (var r = 0; r < matrix.Length; r++) matrix[r][0] = 0;
    }
}
