namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestRectangleInHistogram;

public sealed class LargestRectangleInHistogramTests
{
    [Theory]
    [InlineData(new[] { 2,1,5,6,2,3 }, 10)]
    [InlineData(new[] { 2,4 }, 4)]
    [InlineData(new[] { 1,1 }, 2)]
    public void LargestRectangleArea_MonotonicStack_ReturnsBestArea(int[] heights, int expected) => Assert.Equal(expected, LargestRectangleArea(heights));

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
