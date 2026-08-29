namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixII;

public sealed class SpiralMatrixIITests
{
    [Fact]
    public void GenerateMatrix_FillsBoundsClockwise_ReturnsSpiral()
    {
        Assert.Equal([[1,2,3],[8,9,4],[7,6,5]], GenerateMatrix(3));
    }

    private static int[][] GenerateMatrix(int n)
    {
        var matrix = Enumerable.Range(0, n).Select(_ => new int[n]).ToArray();
        var value = 1; var top = 0; var bottom = n - 1; var left = 0; var right = n - 1;
        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++) matrix[top][c] = value++; top++;
            for (var r = top; r <= bottom; r++) matrix[r][right] = value++; right--;
            for (var c = right; c >= left; c--) matrix[bottom][c] = value++; bottom--;
            for (var r = bottom; r >= top; r--) matrix[r][left] = value++; left++;
        }
        return matrix;
    }
}
