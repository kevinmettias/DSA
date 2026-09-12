using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SubarrayProductLessThanK;

// LeetCode 713. Subarray Product Less Than K: count contiguous subarrays whose
// product of elements is strictly less than k. nums are all positive
// (1 <= nums[i]), so a running SUM of logs is monotonically non-decreasing - the
// exact "derived monotonic sequence" shape BinarySearch.LowerBound assumes, the
// same idiom this repo's LC 209 (Minimum Size Subarray Sum) composition already
// establishes for its own prefix-sum array (there summing values directly; here
// summing logs, since the raw running PRODUCT overflows almost immediately at
// realistic lengths - up to 1000 per element).
internal static class SubarrayProductLessThanKSolution
{
    // The textbook O(n^2): for every start index, extend the running product
    // right until it stops being < k. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static int NumSubarrayProductLessThanKByBruteForce(int[] nums, int k)
    {
        if (k <= 1)
        {
            return 0;
        }

        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            long product = 1;

            for (var end = start; end < nums.Length; end++)
            {
                product *= nums[end];

                if (product >= k)
                {
                    break;
                }

                count++;
            }
        }

        return count;
    }

    // For each start index i, LowerBound finds the first end index m whose
    // cumulative log-sum reaches logPrefix[i] + log(k); every end index in
    // [i+1, m) is a valid subarray (product < k), so m - i - 1 is the count of
    // valid subarrays starting at i. O(n log n).
    public static int NumSubarrayProductLessThanKByLogPrefixLowerBound(int[] nums, int k)
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
        var logK = Math.Log(k);

        for (var i = 0; i < nums.Length; i++)
        {
            var target = logPrefix[i] + logK;
            var end = BinarySearch.LowerBound<double, ArraySequence<double>>(sequence, target);
            count += Math.Max(0, end - i - 1);
        }

        return count;
    }
}
