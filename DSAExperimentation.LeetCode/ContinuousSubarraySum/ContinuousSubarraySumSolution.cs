using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ContinuousSubarraySum;

// LeetCode 523. Continuous Subarray Sum: does nums contain a subarray of length >= 2
// whose elements sum to a multiple of k?
internal static class ContinuousSubarraySumSolution
{
    // LeetCode requires the subarray's start and end indices to differ, i.e. at least
    // two elements.
    private const int MinimumSubarrayLength = 2;

    // The textbook answer: every start/end pair, summing as it extends - O(n^2),
    // deliberately without this repo's primitives. The arm the hashmap-prefix-
    // remainder strategy below has to justify itself against.
    public static bool HasSubarraySumMultipleOfKByBruteForce(int[] nums, int k)
    {
        for (var start = 0; start < nums.Length; start++)
        {
            var sum = 0;
            for (var end = start; end < nums.Length; end++)
            {
                sum += nums[end];
                if (end - start >= MinimumSubarrayLength - 1 && sum % k == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // This repo's own HashMap<int,int> (remainder -> earliest index it was first seen
    // at) - the exact "first occurrence index" shape ContainsDuplicateII already uses,
    // just keyed by running-sum remainder instead of by value. Two indices sharing a
    // remainder means the subarray between them sums to a multiple of k; seeding the
    // map with {0: -1} lets a subarray starting at index 0 register the same way as
    // any other.
    public static bool HasSubarraySumMultipleOfKByHashMapPrefixRemainder(int[] nums, int k)
    {
        var firstIndexByRemainder = new HashMap<int, int>();
        firstIndexByRemainder.Set(0, -1);

        return HasRepeatedRemainder(nums, k, firstIndexByRemainder);
    }

    // Walks the running-sum remainders against their first-occurrence index: a remainder
    // seen before at `firstIndex` closes a subarray of at least MinimumSubarrayLength.
    private static bool HasRepeatedRemainder(int[] nums, int k, HashMap<int, int> firstIndexByRemainder)
    {
        var prefixSum = 0;
        for (var i = 0; i < nums.Length; i++)
        {
            prefixSum += nums[i];
            var remainder = prefixSum % k;

            if (firstIndexByRemainder.TryGetValue(remainder, out var firstIndex))
            {
                if (i - firstIndex >= MinimumSubarrayLength)
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
