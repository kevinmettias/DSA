using RainStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.TrappingRainWater;

// LeetCode 42. Trapping Rain Water: sum, over every bar, the water trapped
// above it - min(tallest wall to its left, tallest wall to its right) minus
// its own height, whenever that is positive.
//
// The two strategies differ only in how each bar finds its bounding walls:
// rescanning the whole array left and right from scratch (the textbook
// O(n^2) approach), or a single left-to-right sweep using this repo's own
// Stack<int> to hold indices of bars still waiting for a taller bar on their
// right, so each bar is bounded exactly once.
internal static class TrappingRainWaterSolution
{
    // The textbook brute force: for every bar, rescan the whole array on both
    // sides for its tallest wall. Deliberately written without this repo's
    // primitives - it is the arm the sweep below has to justify itself against.
    public static int TrapByBruteForce(int[] height)
    {
        var water = 0;

        for (var i = 0; i < height.Length; i++)
        {
            var leftMax = 0;
            for (var l = 0; l <= i; l++)
            {
                leftMax = Math.Max(leftMax, height[l]);
            }

            var rightMax = 0;
            for (var r = i; r < height.Length; r++)
            {
                rightMax = Math.Max(rightMax, height[r]);
            }

            water += Math.Min(leftMax, rightMax) - height[i];
        }

        return water;
    }

    // Single left-to-right sweep. The stack holds bar indices in decreasing
    // height order; once a taller bar arrives, every shorter bar popped off
    // the top had its floor bounded on the left by the new stack top and on
    // the right by the current bar - min(leftWall, rightWall) - floor, times
    // the gap width, is exactly the water that bar's position trapped.
    public static int TrapByMonotonicStack(int[] height)
    {
        var indices = new RainStack();
        var water = 0;

        for (var i = 0; i < height.Length; i++)
        {
            while (indices.TryPeek(out var top) && height[top] < height[i])
            {
                var trapped = TryPopAndComputeBoundedWater(indices, height, i);

                if (trapped is null)
                {
                    break;
                }

                water += trapped.Value;
            }

            indices.Push(i);
        }

        return water;
    }

    private static int? TryPopAndComputeBoundedWater(RainStack indices, int[] height, int barIndex)
    {
        indices.TryPop(out var top);

        if (!indices.TryPeek(out var left))
        {
            return null;
        }

        return BoundedWaterAbove(height, top, left, barIndex);
    }

    // The water a popped bar holds: its floor is the bar itself, its left wall the
    // stack top left behind by the pop, its right wall the current bar - so
    // min(leftWall, rightWall) - floor, times the gap width between the walls.
    private static int BoundedWaterAbove(int[] height, int bar, int leftWall, int rightWall) =>
        (rightWall - leftWall - 1) * (Math.Min(height[leftWall], height[rightWall]) - height[bar]);
}
