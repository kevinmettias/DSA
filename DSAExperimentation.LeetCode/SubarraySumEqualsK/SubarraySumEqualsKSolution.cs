using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.SubarraySumEqualsK;

// LeetCode 560. Subarray Sum Equals K: count subarrays summing to k. A subarray
// nums[i+1..j] sums to k exactly when prefixSum[j] - prefixSum[i] == k, and several
// earlier indices can share the same prefix sum, so the COUNT (not just existence)
// of each earlier prefix sum must be tracked - the same running-prefix-sum move
// ContinuousSubarraySumSolution already makes with this repo's own HashMap<int,int>.
internal static class SubarraySumEqualsKSolution
{
    // Baseline: the textbook O(n^2) double loop over every (start, end) pair. BCL
    // only internally - no repo container beyond the caller-supplied array.
    public static int CountByBruteForce(int[] nums, int k)
    {
        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var sum = 0;
            for (var end = start; end < nums.Length; end++)
            {
                sum += nums[end];
                if (sum == k)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public static int CountByPrefixSumHashMap(int[] nums, int k)
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
