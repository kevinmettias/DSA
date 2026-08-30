using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumWidthRamp;

// LeetCode 962. Maximum Width Ramp: build a monotonically-decreasing candidate
// stack of left-endpoint indices over this repo's own Stack<int> (CarFleet/
// DailyTemperatures precedent for this repo's own Stack instead of the CLR's
// System.Collections.Generic.Stack), then walk right-to-left popping every
// candidate whose value is <= the current right endpoint - each pop is a
// widest-so-far ramp for that candidate, since indices only ever get more
// favorable (further right) as the scan proceeds.
public sealed class MaximumWidthRampTests
{
    [Fact]
    public void MaxWidthRamp_ClassicExample_ReturnsFour()
    {
        int[] nums = [6, 0, 8, 2, 1, 5];

        Assert.Equal(4, MaxWidthRamp(nums));
    }

    [Fact]
    public void MaxWidthRamp_LongerExampleWithRepeatedValues_ReturnsSeven()
    {
        int[] nums = [9, 8, 1, 0, 1, 9, 4, 0, 4, 1];

        Assert.Equal(7, MaxWidthRamp(nums));
    }

    [Fact]
    public void MaxWidthRamp_StrictlyDecreasing_ReturnsZero()
    {
        int[] nums = [5, 4, 3, 2, 1];

        Assert.Equal(0, MaxWidthRamp(nums));
    }

    private static int MaxWidthRamp(int[] nums)
    {
        var candidates = new RepoIndexStack();

        for (var i = 0; i < nums.Length; i++)
        {
            if (!candidates.TryPeek(out var topIndex) || nums[topIndex] > nums[i])
            {
                candidates.Push(i);
            }
        }

        var maxWidth = 0;

        for (var j = nums.Length - 1; j >= 0; j--)
        {
            while (candidates.TryPeek(out var topIndex) && nums[topIndex] <= nums[j])
            {
                candidates.TryPop(out _);
                maxWidth = Math.Max(maxWidth, j - topIndex);
            }
        }

        return maxWidth;
    }
}
