namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrix;

public sealed class SpiralMatrixTests
{
    [Fact]
    public void SpiralOrder_WalksBoundsClockwise_ReturnsAllValues()
    {
        int[][] matrix = [[1,2,3],[4,5,6],[7,8,9]];
        Assert.Equal([1,2,3,6,9,8,7,4,5], SpiralOrder(matrix));
    }

    private static IList<int> SpiralOrder(int[][] matrix)
    {
        var result = new List<int>(); var top = 0; var bottom = matrix.Length - 1; var left = 0; var right = matrix[0].Length - 1;
        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++) result.Add(matrix[top][c]); top++;
            for (var r = top; r <= bottom; r++) result.Add(matrix[r][right]); right--;
            if (top <= bottom) { for (var c = right; c >= left; c--) result.Add(matrix[bottom][c]); bottom--; }
            if (left <= right) { for (var r = bottom; r >= top; r--) result.Add(matrix[r][left]); left++; }
        }
        return result;
    }
}
