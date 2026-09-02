using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContinuousSubarraySum;

// LeetCode 523. Continuous Subarray Sum: prefix-sum-mod-k tracked in this repo's own
// HashMap<int,int> (remainder -> earliest index it was first seen at) - the exact
// "first occurrence index" shape ContainsDuplicateII already uses, just keyed by
// running-sum remainder instead of by value. Two indices sharing a remainder means
// the subarray between them sums to a multiple of k; seeding the map with {0: -1}
// lets a subarray starting at index 0 register the same way as any other.
public sealed partial class ContinuousSubarraySumTests
{
    [Theory]
    [InlineData(new[] { 23, 2, 4, 6, 7 }, 6, true)]
    [InlineData(new[] { 23, 2, 6, 4, 7 }, 6, true)]
    [InlineData(new[] { 23, 2, 6, 4, 7 }, 13, false)]
    public void HasContinuousSubarraySum_LeetCodeExamples_ReturnsExpected(int[] nums, int k, bool expected)
    {
        var hasSubarraySum = HasSubarraySumMultipleOfK(nums, k);
        Assert.Equal(expected, hasSubarraySum);
    }

    private static bool HasSubarraySumMultipleOfK(int[] nums, int k)
    {
        var firstIndexByRemainder = new HashMap<int, int>();
        firstIndexByRemainder.Set(0, -1);

        return HasQualifyingSubarray(nums, k, firstIndexByRemainder);
    }

    private static bool HasQualifyingSubarray(int[] nums, int k, HashMap<int, int> firstIndexByRemainder)
    {
        var prefixSum = 0;
        for (var i = 0; i < nums.Length; i++)
        {
            prefixSum += nums[i];
            var remainder = prefixSum % k;

            if (firstIndexByRemainder.TryGetValue(remainder, out var firstIndex))
            {
                if (i - firstIndex >= 2)
                {
                    return true;
                }
            }
            else
            {
                firstIndexByRemainder.Set(remainder, i);
            }
        }

        return false;
    }
}
