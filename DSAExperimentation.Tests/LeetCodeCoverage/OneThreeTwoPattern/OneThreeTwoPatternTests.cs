using OneThreeTwoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OneThreeTwoPattern;

// LeetCode 456. 132 Pattern: the classic right-to-left monotonic-stack sweep over
// this repo's own Stack<int> - the same LargestRectangleInHistogramTests/
// TrappingRainWaterTests precedent - holding a decreasing stack of "2" candidates
// while popping every value smaller than the current one into "third" (the best
// possible "2" seen so far, paired with whatever "3" popped it). A later, further-
// left value smaller than "third" proves a valid 1 < 3-candidate... < 2 exists.
public sealed partial class OneThreeTwoPatternTests
{
    [Fact]
    public void Find132Pattern_StrictlyIncreasing_ReturnsFalse()
    {
        int[] nums = [1, 2, 3, 4];

        Assert.False(Find132Pattern(nums));
    }

    [Fact]
    public void Find132Pattern_ClassicExample_ReturnsTrue()
    {
        int[] nums = [3, 1, 4, 2];

        Assert.True(Find132Pattern(nums));
    }

    [Fact]
    public void Find132Pattern_NegativeAndZeroValues_ReturnsTrue()
    {
        int[] nums = [-1, 3, 2, 0];

        Assert.True(Find132Pattern(nums));
    }

    private static bool Find132Pattern(int[] nums)
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
