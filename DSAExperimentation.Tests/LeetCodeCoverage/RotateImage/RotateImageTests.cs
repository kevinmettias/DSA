namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateImage;

public sealed class RotateImageTests
{
    [Fact]
    public void Rotate_TransposeAndReverseRows_RotatesClockwise()
    {
        int[][] matrix = [[1,2,3],[4,5,6],[7,8,9]];
        Rotate(matrix);
        Assert.Equal([[7,4,1],[8,5,2],[9,6,3]], matrix);
    }

    private static void Rotate(int[][] matrix)
    {
        var n = matrix.Length;
        for (var r = 0; r < n; r++) for (var c = r + 1; c < n; c++) (matrix[r][c], matrix[c][r]) = (matrix[c][r], matrix[r][c]);
        foreach (var row in matrix) Array.Reverse(row);
    }
}
