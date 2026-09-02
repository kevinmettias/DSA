using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarraySumEqualsK;

// LeetCode 560. Subarray Sum Equals K: the same running-prefix-sum move
// ContinuousSubarraySumTests already uses this repo's own HashMap<int,int> for, just
// counting every earlier occurrence of (prefixSum - k) instead of only the first one -
// a subarray sums to k exactly when two prefix sums differ by k, and several earlier
// indices can share the same prefix sum.
public sealed partial class SubarraySumEqualsKTests
{
    [Fact]
    public void CountSubarrays_ClassicExample_ReturnsTwoMatchingSubarrays()
    {
        int[] nums = [1, 1, 1];

        var count = CountSubarraysSummingToK(nums, k: 2);

        Assert.Equal(2, count);
    }

    [Fact]
    public void CountSubarrays_WithMultipleOverlappingMatches_CountsEveryOne()
    {
        int[] nums = [1, 2, 1, 2, 1];

        var count = CountSubarraysSummingToK(nums, k: 3);

        Assert.Equal(4, count);
    }

    [Fact]
    public void CountSubarrays_WithNegativeNumbers_StillFindsMatchingSubarrays()
    {
        int[] nums = [1, -1, 0];

        var count = CountSubarraysSummingToK(nums, k: 0);

        Assert.Equal(3, count);
    }

    private static int CountSubarraysSummingToK(int[] nums, int k)
    {
        var countByPrefixSum = new HashMap<int, int>();
        countByPrefixSum.Set(0, 1);

        var prefixSum = 0;
        var count = 0;

        foreach (var num in nums)
        {
            prefixSum += num;

            if (countByPrefixSum.TryGetValue(prefixSum - k, out var matches))
            {
                count += matches;
            }

            countByPrefixSum.TryGetValue(prefixSum, out var existingCount);
            countByPrefixSum.Set(prefixSum, existingCount + 1);
        }

        return count;
    }
}
