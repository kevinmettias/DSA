using DSAExperimentation.Domain.Modular;
using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

namespace DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsI;

// LeetCode 3250. Find the Count of Monotonic Pairs I: count pairs of non-negative
// integer arrays (arr1, arr2), both length n, with arr1 non-decreasing, arr2
// non-increasing, and arr1[i] + arr2[i] == nums[i] for every i - modulo 1e9+7.
//
// arr2 is entirely determined by arr1 (arr2 = nums - arr1), so the only free
// choice is arr1. Two constraints on arr1 fall out of the definition:
// 0 <= arr1[i] <= nums[i] (arr2[i] >= 0), and, since arr2 must not increase,
// arr1[i] >= arr1[i-1] + max(0, nums[i] - nums[i-1]) (nums[i]-arr1[i] <=
// nums[i-1]-arr1[i-1]). Counting length-n sequences under that recurrence is a
// row-by-row DP over "arr1[i] == j": row i is the prefix sum of row i-1, shifted
// right by that row's own delta = max(0, nums[i]-nums[i-1]).
internal static class FindTheCountOfMonotonicPairsISolution
{
    // Every row re-sums its prefix from scratch for each j instead of carrying a
    // running total - O(n * maxValue^2), the DP the recurrence gives you before
    // noticing each row is itself a running sum of the row before it. Part I's
    // own constraint (nums[i] <= 50) keeps this tractable; Part II's larger
    // maxValue is exactly why the prefix-sum arm below exists.
    public static long CountPairsByBruteForceDP(int[] nums)
    {
        var maxValue = nums.Max();
        var previousRow = new long[maxValue + 1];

        for (var value = 0; value <= nums[0]; value++)
        {
            previousRow[value] = 1;
        }

        for (var i = 1; i < nums.Length; i++)
        {
            previousRow = SumRowByRescan(previousRow, nums, i, maxValue);
        }

        return Total(previousRow);
    }

    // One brute-force DP row: row i is the sum of row i-1 restricted to arr1[i] == j,
    // which here means re-summing every previousValue that clears this row's delta.
    private static long[] SumRowByRescan(long[] previousRow, int[] nums, int index, int maxValue)
    {
        var delta = Math.Max(0, nums[index] - nums[index - 1]);
        var currentRow = new long[maxValue + 1];

        for (var j = 0; j <= nums[index]; j++)
        {
            var limit = Math.Min(j - delta, nums[index - 1]);
            var sum = 0L;

            for (var previousValue = 0; previousValue <= limit; previousValue++)
            {
                sum += previousRow[previousValue];
            }

            currentRow[j] = sum % ModularArithmetic.Modulo;
        }

        return currentRow;
    }

    // The same recurrence, but each row is built from a running prefix sum of the
    // row before it, so every currentRow[j] is one lookup instead of a re-summed
    // loop - O(n * maxValue) overall. That O(n * maxValue) form is what Part II's
    // larger maxValue actually needs, which is why LC 3251's class holds its one
    // implementation; this arm calls through. Nothing narrows - both parts answer
    // a long.
    public static long CountPairsByPrefixSumDP(int[] nums) =>
        FindTheCountOfMonotonicPairsIISolution.CountPairsByPrefixSumDP(nums);

    private static long Total(long[] lastRow)
    {
        var total = 0L;

        foreach (var count in lastRow)
        {
            total = (total + count) % ModularArithmetic.Modulo;
        }

        return total;
    }
}
