using HistogramStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.LargestRectangleInHistogram;

// LeetCode 84. Largest Rectangle in Histogram: the widest rectangle that fits
// under a contiguous run of bars, taking the shortest bar in the run as its
// height.
//
// The two strategies differ only in how each bar finds the width it could
// span: rescanning outward from scratch for every bar (the textbook O(n^2)
// approach), or a single left-to-right sweep using this repo's own
// Stack<int> to hold indices of bars still waiting for a shorter bar to end
// their run, so each bar's span is resolved exactly once - the same
// TrappingRainWaterSolution precedent (Stack<int> of indices), applied to
// histogram area instead of trapped water.
internal static class LargestRectangleInHistogramSolution
{
    // The textbook brute force: for every bar, expand outward in both
    // directions taking the running minimum height, and track the best area
    // seen. Deliberately written without this repo's primitives - it is the
    // arm the sweep below has to justify itself against.
    public static int LargestRectangleAreaByBruteForce(int[] heights)
    {
        var maxArea = 0;

        for (var i = 0; i < heights.Length; i++)
        {
            var minHeight = heights[i];

            for (var j = i; j < heights.Length; j++)
            {
                minHeight = Math.Min(minHeight, heights[j]);
                maxArea = Math.Max(maxArea, minHeight * (j - i + 1));
            }
        }

        return maxArea;
    }

    // Single left-to-right sweep. The stack holds bar indices in increasing
    // height order; once a bar shorter than the current top arrives, every
    // taller bar popped off the top had its run bounded on the right by the
    // current index and on the left by the new stack top (or the start of
    // the array, if nothing is left) - height * width is exactly the largest
    // rectangle that popped bar could anchor. A sentinel pass past the end
    // (height 0) flushes every bar still on the stack once, so no separate
    // drain loop is needed after the main sweep.
    public static int LargestRectangleAreaByMonotonicStack(int[] heights)
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
