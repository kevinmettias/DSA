namespace DSAExperimentation.LeetCode.MaximumSubarray;

// LeetCode 53. Maximum Subarray: the largest sum of any non-empty contiguous
// run of nums. No repo primitive applies - this is a pure running-best scan
// over the array itself, the same "no stronger reusable primitive" shape
// already established for BestTimeToBuyAndSellStock/GasStation.
internal static class MaximumSubarraySolution
{
    // The textbook answer: sum every contiguous subarray and keep the best,
    // O(n^2) - the arm Kadane's single pass below has to justify itself
    // against.
    public static int MaxSubArrayByBruteForce(int[] nums)
    {
        var best = nums[0];

        for (var i = 0; i < nums.Length; i++)
        {
            var sum = 0;

            for (var j = i; j < nums.Length; j++)
            {
                sum += nums[j];
                best = Math.Max(best, sum);
            }
        }

        return best;
    }

    // Kadane's scan: at each position, either extend the running subarray or
    // restart it there, keeping the best sum seen - O(n), one pass.
    public static int MaxSubArrayByKadaneScan(int[] nums)
    {
        var best = nums[0];
        var current = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            current = Math.Max(nums[i], current + nums[i]);
            best = Math.Max(best, current);
        }

        return best;
    }
}
