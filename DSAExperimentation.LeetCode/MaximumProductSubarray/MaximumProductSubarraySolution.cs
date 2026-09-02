namespace DSAExperimentation.LeetCode.MaximumProductSubarray;

// LeetCode 152. Maximum Product Subarray: the largest product of a contiguous
// subarray. A negative value can turn the running minimum product into the new
// running maximum (and vice versa), so both must be tracked while scanning left
// to right, not just a running best.
//
// LeetCode's own signature returns int - its constraint guarantees the product
// of any subarray fits in a 32-bit integer - so both strategies report int. The
// running accumulators stay long internally so a synthetic benchmark workload
// that does not respect that guarantee cannot silently wrap into a wrong
// comparison baseline; the original test arm tracked int and the original
// benchmark arm tracked long for the same computation, and long is the one that
// cannot misbehave under the larger [Params] size, so it wins.
internal static class MaximumProductSubarraySolution
{
    // The textbook answer: every subarray's product, multiplied out from
    // scratch. O(n^2).
    public static int MaxProductByBruteForce(int[] nums)
    {
        long best = nums[0];

        for (var i = 0; i < nums.Length; i++)
        {
            long product = 1;

            for (var j = i; j < nums.Length; j++)
            {
                product *= nums[j];
                best = Math.Max(best, product);
            }
        }

        return (int)best;
    }

    // One O(n) pass: a negative nums[i] swaps the roles of the running max and
    // running min product ending at i, because the smallest (most negative)
    // product so far can become the largest once multiplied by a negative
    // number - so both must be carried forward together.
    public static int MaxProductByMinMaxScan(int[] nums)
    {
        long min = nums[0];
        long max = nums[0];
        long best = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            if (nums[i] < 0)
            {
                (min, max) = (max, min);
            }

            max = Math.Max(nums[i], max * nums[i]);
            min = Math.Min(nums[i], min * nums[i]);
            best = Math.Max(best, max);
        }

        return (int)best;
    }
}
