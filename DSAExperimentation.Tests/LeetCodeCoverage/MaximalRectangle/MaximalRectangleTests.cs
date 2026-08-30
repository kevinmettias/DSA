using HeightStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximalRectangle;

// LeetCode 85. Maximal Rectangle: reduces to LeetCode 84 (Largest Rectangle in
// Histogram) applied once per row - each row's running height array (consecutive
// '1's stacked on top of the row above) turns the 2D search into rowCount
// independent histogram-max-rectangle sweeps, reusing the identical Stack<int>
// monotonic-stack routine LargestRectangleInHistogramTests already proves out.
public sealed partial class MaximalRectangleTests
{
    [Fact]
    public void MaximalRectangleArea_ClassicExample_ReturnsLargestAllOnesRectangle()
    {
        char[][] matrix =
        [
            ['1', '0', '1', '0', '0'],
            ['1', '0', '1', '1', '1'],
            ['1', '1', '1', '1', '1'],
            ['1', '0', '0', '1', '0'],
        ];

        Assert.Equal(6, MaximalRectangleArea(matrix));
    }

    [Fact]
    public void MaximalRectangleArea_AllZeros_ReturnsZero()
    {
        char[][] matrix = [['0', '0'], ['0', '0']];

        Assert.Equal(0, MaximalRectangleArea(matrix));
    }

    private static int MaximalRectangleArea(char[][] matrix)
    {
        if (matrix.Length == 0)
        {
            return 0;
        }

        var heights = new int[matrix[0].Length];
        var maxArea = 0;

        foreach (var row in matrix)
        {
            for (var col = 0; col < row.Length; col++)
            {
                heights[col] = row[col] == '1' ? heights[col] + 1 : 0;
            }

            maxArea = Math.Max(maxArea, LargestRectangleArea(heights));
        }

        return maxArea;
    }

    private static int LargestRectangleArea(int[] heights)
    {
        var indices = new HeightStack();
        var maxArea = 0;

        for (var i = 0; i <= heights.Length; i++)
        {
            var currentHeight = i == heights.Length ? 0 : heights[i];

            while (indices.TryPeek(out var top) && heights[top] >= currentHeight)
            {
                indices.TryPop(out _);
                var height = heights[top];
                var width = indices.TryPeek(out var left) ? i - left - 1 : i;
                maxArea = Math.Max(maxArea, height * width);
            }

            indices.Push(i);
        }

        return maxArea;
    }
}
