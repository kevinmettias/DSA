namespace DSAExperimentation.LeetCode.MaximumSumCircularSubarray;

// LeetCode 918. Maximum Sum Circular Subarray: the largest sum of any non-empty
// subarray of a circular array, where a subarray may wrap past the end and back to
// the front (but may still use each element at most once). No repo primitive
// applies - this is a running-best scan over the array itself, the same "no
// stronger reusable primitive" shape MaximumSubarraySolution already established
// for LC 53's linear case.
internal static class MaximumSumCircularSubarraySolution
{
    // The textbook answer: try every circular subarray - every start index, every
    // length 1..n, wrapping via modulo - and keep the best sum, O(n^2). This is the
    // arm the two-pass Kadane trick below has to justify itself against.
    public static int MaxSubarraySumCircularByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var best = nums[0];

        for (var start = 0; start < n; start++)
        {
            var sum = 0;

            for (var length = 1; length <= n; length++)
            {
                sum += nums[(start + length - 1) % n];
                best = Math.Max(best, sum);
            }
        }

        return best;
    }

    // MaximumSubarraySolution's own Kadane scan (LC 53's precedent), run twice over
    // the same array - once for the best NON-wraparound subarray sum (unchanged),
    // once for the WORST subarray sum (the same recurrence with Math.Min instead of
    // Math.Max) - so the best WRAPAROUND subarray reduces to total - worstSum, the
    // standard complement trick (a wraparound subarray is exactly "everything
    // except some non-wraparound middle stretch"). All-negative input is the one
    // case where that complement is wrong (it would credit an empty wraparound
    // subarray, which isn't a legal answer), caught by falling back to the plain
    // maxSum whenever it's still negative. O(n), one pass.
    public static int MaxSubarraySumCircularByTwoPassKadane(int[] nums)
    {
        var total = 0;
        var maxSum = nums[0];
        var currentMax = 0;
        var minSum = nums[0];
        var currentMin = 0;

        foreach (var n in nums)
        {
            currentMax = Math.Max(n, currentMax + n);
            maxSum = Math.Max(maxSum, currentMax);

            currentMin = Math.Min(n, currentMin + n);
            minSum = Math.Min(minSum, currentMin);

            total += n;
        }

        return maxSum < 0 ? maxSum : Math.Max(maxSum, total - minSum);
    }
}
