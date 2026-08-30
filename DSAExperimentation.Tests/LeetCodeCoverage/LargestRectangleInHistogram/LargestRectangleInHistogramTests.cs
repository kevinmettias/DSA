using HistogramStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestRectangleInHistogram;

// LeetCode 84. Largest Rectangle in Histogram: the classic monotonic-stack sweep
// over this repo's own Stack<int>, holding bar indices (not heights) - the same
// TrappingRainWaterTests precedent, applied to histogram area instead of trapped
// water. A sentinel pass past the end (height 0) flushes every bar still on the
// stack once, so no separate drain loop is needed after the main sweep.
public sealed partial class LargestRectangleInHistogramTests
{
    [Fact]
    public void LargestRectangleArea_ClassicExample_ReturnsMaxArea()
    {
        int[] heights = [2, 1, 5, 6, 2, 3];

        Assert.Equal(10, LargestRectangleArea(heights));
    }

    [Fact]
    public void LargestRectangleArea_StrictlyIncreasingBars_ReturnsBestSuffixRectangle()
    {
        int[] heights = [1, 2, 3, 4, 5];

        Assert.Equal(9, LargestRectangleArea(heights));
    }

    private static int LargestRectangleArea(int[] heights)
    {
        var indices = new HistogramStack();
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
