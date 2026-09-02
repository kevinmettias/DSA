using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarrayProductLessThanK;

// LeetCode 713. Subarray Product Less Than K: nums are all positive (1 <= nums[i]),
// so a running SUM of logs is monotonically non-decreasing - the exact "derived
// monotonic sequence" shape BinarySearch.LowerBound assumes, the same idiom
// MinimumSizeSubarraySumTests (LC 209) already established for its own prefix-sum
// array (there summing values directly; here summing logs, since the raw running
// PRODUCT overflows almost immediately - up to 1000 per element). For each start
// index i, LowerBound finds the first end index m whose cumulative log-sum reaches
// logPrefix[i] + log(k); every end index in [i+1, m) is a valid subarray (product
// < k), so m - i - 1 is the count of valid subarrays starting at i. The textbook
// O(n) two-pointer sliding window needs no data structure at all (same reason LC11
// Container With Most Water stayed blocked), so this LowerBound composition is the
// version that is a genuine primitive composition rather than an ad hoc scan.
public sealed partial class SubarrayProductLessThanKTests
{
    [Fact]
    public void NumSubarrayProductLessThanK_LeetCodeExample_ReturnsEightValidSubarrays()
    {
        var count = NumSubarrayProductLessThanK([10, 5, 2, 6], k: 100);

        Assert.Equal(8, count);
    }

    [Fact]
    public void NumSubarrayProductLessThanK_KAtMostOne_ReturnsZero()
    {
        var count = NumSubarrayProductLessThanK([1, 2, 3], k: 0);

        Assert.Equal(0, count);
    }

    [Fact]
    public void NumSubarrayProductLessThanK_SingleElementBelowK_CountsItself()
    {
        var count = NumSubarrayProductLessThanK([5], k: 10);

        Assert.Equal(1, count);
    }

    private static int NumSubarrayProductLessThanK(int[] nums, int k)
    {
        if (k <= 1)
        {
            return 0;
        }

        var logPrefix = new double[nums.Length + 1];
        for (var i = 0; i < nums.Length; i++)
        {
            logPrefix[i + 1] = logPrefix[i] + Math.Log(nums[i]);
        }

        var sequence = new ArraySequence<double>(logPrefix);
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            var target = logPrefix[i] + Math.Log(k);
            var end = BinarySearch.LowerBound<double, ArraySequence<double>>(sequence, target);
            count += Math.Max(0, end - i - 1);
        }

        return count;
    }
}
