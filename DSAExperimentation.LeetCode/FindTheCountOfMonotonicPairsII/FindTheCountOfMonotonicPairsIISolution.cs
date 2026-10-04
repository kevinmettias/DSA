using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

// LeetCode 3251. Find the Count of Monotonic Pairs II: the exact same question as
// Part I (LeetCode 3250) - count pairs of non-negative integer arrays (arr1, arr2)
// with arr1 non-decreasing, arr2 non-increasing, arr1[i] + arr2[i] == nums[i] for
// every i, modulo 1e9+7 - just with nums[i] allowed up to 1000 instead of 50.
//
// arr2 is entirely determined by arr1 (arr2 = nums - arr1), so the only free
// choice is arr1. Two constraints on arr1 fall out of the definition:
// 0 <= arr1[i] <= nums[i] (arr2[i] >= 0), and, since arr2 must not increase,
// arr1[i] >= arr1[i-1] + max(0, nums[i] - nums[i-1]) (nums[i]-arr1[i] <=
// nums[i-1]-arr1[i-1]). Counting length-n sequences under that recurrence is a
// row-by-row DP over "arr1[i] == j": row i is the prefix sum of row i-1, shifted
// right by that row's own delta = max(0, nums[i]-nums[i-1]).
//
// This class owns both arms, and Part I's class calls through to them (ARCHITECTURE
// 17.3): the wider bound is the one whose answer covers the narrower, and both parts
// answer a long, so nothing narrows. The bigger maxValue is why this problem exists
// as a separate LeetCode entry: the O(n * maxValue^2) brute-force row is still
// correct here, just no longer the arm you'd ship - O(n * maxValue) prefix sums is.
internal static class FindTheCountOfMonotonicPairsIISolution
{
    // Textbook baseline: every row re-sums its prefix from scratch for each j instead
    // of carrying a running total - O(n * maxValue^2), the DP the recurrence gives you
    // before noticing each row is itself a running sum of the row before it. Part I's
    // bound (nums[i] <= 50) keeps it tractable; this part's 1000 is why the prefix-sum
    // arm below exists.
    public static long CountPairsByBruteForceDP(int[] nums)
    {
        var maxValue = nums.Max();
        var previousRow = FirstRow(nums, maxValue);

        for (var i = 1; i < nums.Length; i++)
        {
            previousRow = SumRowByRescan(previousRow, nums, i, maxValue);
        }

        return Total(previousRow);
    }

    // A running prefix sum turns each row into O(maxValue) instead of
    // O(maxValue^2) - O(n * maxValue) overall, which is what keeps this
    // tractable once nums[i] reaches 1000. Domain.Modular.ModularArithmetic
    // supplies the 1e9+7 both Monotonic Pairs problems report under.
    public static long CountPairsByPrefixSumDP(int[] nums)
    {
        var maxValue = nums.Max();
        var previousRow = FirstRow(nums, maxValue);

        for (var i = 1; i < nums.Length; i++)
        {
            previousRow = BuildRowByPrefixSum(previousRow, nums, i, maxValue);
        }

        return Total(previousRow);
    }

    // Row 0: arr1[0] may be any value from 0 to nums[0], each in exactly one way.
    private static long[] FirstRow(int[] nums, int maxValue)
    {
        var row = new long[maxValue + 1];

        for (var value = 0; value <= nums[0]; value++)
        {
            row[value] = 1;
        }

        return row;
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

    // The prefix-sum twin of SumRowByRescan: the same row, but each entry is read out
    // of one upfront running-prefix pass over previousRow instead of re-summed.
    private static long[] BuildRowByPrefixSum(long[] previousRow, int[] nums, int rowIndex, int maxValue)
    {
        var delta = Math.Max(0, nums[rowIndex] - nums[rowIndex - 1]);
        var prefix = PrefixSums(previousRow, nums[rowIndex - 1]);
        var currentRow = new long[maxValue + 1];

        for (var j = 0; j <= nums[rowIndex]; j++)
        {
            var limit = j - delta;
            currentRow[j] = limit < 0 ? 0 : PrefixAt(prefix, limit, nums[rowIndex - 1]);
        }

        return currentRow;
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

    // The prefix sum at the limit, clamped to the largest index this row's prefix covers.
    private static long PrefixAt(long[] prefix, int limit, int upperInclusive) =>
        prefix[Math.Min(limit, upperInclusive)];

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
