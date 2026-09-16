using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SubarrayProductLessThanK;

// LeetCode 713. Subarray Product Less Than K: count contiguous subarrays whose
// product of elements is strictly less than the product limit. nums are all positive
// (1 <= nums[i]), so a running SUM of logs is monotonically non-decreasing - the
// exact "derived monotonic sequence" shape BinarySearch.LowerBound assumes, the
// same idiom this repo's LC 209 (Minimum Size Subarray Sum) composition already
// establishes for its own prefix-sum array (there summing values directly; here
// summing logs, since the raw running PRODUCT overflows almost immediately at
// realistic lengths - up to 1000 per element).
internal static class SubarrayProductLessThanKSolution
{
    // The textbook O(n^2): for every start index, extend the running product
    // right until it stops being < productLimit. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static int CountSubarraysWithProductLessThanKByBruteForce(int[] nums, int productLimit)
    {
        if (productLimit <= 1)
        {
            return 0;
        }

        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            count += CountEndsFrom(nums, start, productLimit);
        }

        return count;
    }

    // Extends the running product right from `start` until it stops being < productLimit;
    // every end index before that is one valid subarray.
    private static int CountEndsFrom(int[] nums, int start, int productLimit)
    {
        var count = 0;
        long product = 1;

        for (var end = start; end < nums.Length; end++)
        {
            product *= nums[end];

            if (product >= productLimit)
            {
                break;
            }

            count++;
        }

        return count;
    }

    // For each start index i, LowerBound finds the first end index m whose
    // cumulative log-sum reaches logPrefix[i] + log(productLimit); every end index in
    // [i+1, m) is a valid subarray (product < productLimit), so m - i - 1 is the count of
    // valid subarrays starting at i. O(n log n).
    public static int CountSubarraysWithProductLessThanKByLogPrefixLowerBound(int[] nums, int productLimit)
    {
        if (productLimit <= 1)
        {
            return 0;
        }

        var logPrefix = BuildLogPrefix(nums);
        var sequence = new ArraySequence<double>(logPrefix);
        var count = 0;
        var logBound = Math.Log(productLimit);

        for (var i = 0; i < nums.Length; i++)
        {
            var target = logPrefix[i] + logBound;
            var end = BinarySearch.LowerBound<double, ArraySequence<double>>(sequence, target);
            count += Math.Max(0, end - i - 1);
        }

        return count;
    }

    // Cumulative sums of the elements' logs; monotonically non-decreasing because
    // every element is at least 1, which is what LowerBound's own precondition
    // needs of it.
    private static double[] BuildLogPrefix(int[] nums)
    {
        var logPrefix = new double[nums.Length + 1];
        for (var i = 0; i < nums.Length; i++)
        {
            logPrefix[i + 1] = logPrefix[i] + Math.Log(nums[i]);
        }

        return logPrefix;
    }
}
