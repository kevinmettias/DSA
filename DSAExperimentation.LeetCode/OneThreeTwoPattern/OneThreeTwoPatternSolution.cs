using OneThreeTwoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.OneThreeTwoPattern;

// LeetCode 456. 132 Pattern: does nums contain indices i < j < k with
// nums[i] < nums[k] < nums[j]?
//
// The brute force tracks the running prefix minimum as the "1" candidate, then
// rescans everything to its right for a "2,3" pair in O(n^2) - the arm the
// composed solution has to justify itself against. The composed solution sweeps
// right-to-left with a decreasing stack of "2" candidates, popping every value
// smaller than the current one into "third" (the best possible "2" seen so far,
// paired with whatever "3" popped it) - the same LargestRectangleInHistogram/
// TrappingRainWater precedent. A later, further-left value smaller than third
// proves a valid 1 < 3-candidate... < 2 triple exists.
internal static class OneThreeTwoPatternSolution
{
    // The textbook O(n^2) answer, deliberately written without this repo's
    // primitives.
    public static bool HasPatternByBruteForce(int[] nums)
    {
        var minLeft = nums[0];

        for (var j = 1; j < nums.Length; j++)
        {
            for (var k = j + 1; k < nums.Length; k++)
            {
                if (minLeft < nums[k] && nums[k] < nums[j])
                {
                    return true;
                }
            }

            minLeft = Math.Min(minLeft, nums[j]);
        }

        return false;
    }

    // O(n) right-to-left monotonic-stack sweep over this repo's own Stack<int>.
    public static bool HasPatternByMonotonicStack(int[] nums)
    {
        var stack = new OneThreeTwoStack();
        var third = int.MinValue;

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            if (nums[i] < third)
            {
                return true;
            }

            while (stack.TryPeek(out var top) && top < nums[i])
            {
                third = top;
                stack.TryPop(out _);
            }

            stack.Push(nums[i]);
        }

        return false;
    }
}
