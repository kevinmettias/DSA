using DSAExperimentation.Domain.Modular;

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
            var delta = Math.Max(0, nums[i] - nums[i - 1]);
            var currentRow = new long[maxValue + 1];

            for (var j = 0; j <= nums[i]; j++)
            {
                var limit = Math.Min(j - delta, nums[i - 1]);
                var sum = 0L;

                for (var previousValue = 0; previousValue <= limit; previousValue++)
                {
                    sum += previousRow[previousValue];
                }

                currentRow[j] = sum % ModularArithmetic.Modulo;
            }

            previousRow = currentRow;
        }

        return Total(previousRow);
    }

    // The same recurrence, but each row is built from a running prefix sum of the
    // row before it, so every currentRow[j] is one lookup instead of a re-summed
    // loop - O(n * maxValue) overall. This is the arm Part II's larger maxValue
    // actually needs; Domain.Modular.ModularArithmetic supplies the shared
    // 1e9+7 LeetCode reports both parts under.
    public static long CountPairsByPrefixSumDP(int[] nums)
    {
        var maxValue = nums.Max();
        var previousRow = new long[maxValue + 1];

        for (var value = 0; value <= nums[0]; value++)
        {
            previousRow[value] = 1;
        }

        for (var i = 1; i < nums.Length; i++)
        {
            var delta = Math.Max(0, nums[i] - nums[i - 1]);
            var prefix = PrefixSums(previousRow, nums[i - 1]);
            var currentRow = new long[maxValue + 1];

            for (var j = 0; j <= nums[i]; j++)
            {
                var limit = j - delta;
                currentRow[j] = limit < 0 ? 0 : prefix[Math.Min(limit, nums[i - 1])];
            }

            previousRow = currentRow;
        }

        return Total(previousRow);
    }

    private static long[] PrefixSums(long[] row, int upperInclusive)
    {
        var prefix = new long[upperInclusive + 1];
        var running = 0L;

        for (var value = 0; value <= upperInclusive; value++)
        {
            running = (running + row[value]) % ModularArithmetic.Modulo;
            prefix[value] = running;
        }

        return prefix;
    }

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
