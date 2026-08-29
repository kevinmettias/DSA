namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximalRectangle;

public sealed class MaximalRectangleTests
{
    [Fact]
    public void MaximalRectangle_BuildsHistogramRows_ReturnsLargestOneRectangle()
    {
        char[][] matrix = [[ '1','0','1','0','0' ],[ '1','0','1','1','1' ],[ '1','1','1','1','1' ],[ '1','0','0','1','0' ]];
        Assert.Equal(6, MaximalRectangle(matrix));
    }

    private static int MaximalRectangle(char[][] matrix)
    {
        var heights = new int[matrix[0].Length]; var best = 0;
        foreach (var row in matrix)
        {
            for (var c = 0; c < row.Length; c++) heights[c] = row[c] == '1' ? heights[c] + 1 : 0;
            best = Math.Max(best, LargestRectangleArea(heights));
        }
        return best;
    }

    private static int LargestRectangleArea(int[] heights)
    {
        var stack = new Stack<int>(); var best = 0;
        for (var i = 0; i <= heights.Length; i++)
        {
            var current = i == heights.Length ? 0 : heights[i];
            while (stack.Count > 0 && current < heights[stack.Peek()])
            {
                var height = heights[stack.Pop()]; var left = stack.Count == 0 ? -1 : stack.Peek();
                best = Math.Max(best, height * (i - left - 1));
            }
            stack.Push(i);
        }
        return best;
    }
}
