using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

// LeetCode 3251. Find the Count of Monotonic Pairs II: the exact same question as
// Part I (LeetCode 3250) - count pairs of non-negative integer arrays (arr1, arr2)
// with arr1 non-decreasing, arr2 non-increasing, arr1[i] + arr2[i] == nums[i] for
// every i, modulo 1e9+7 - just with nums[i] allowed up to 1000 instead of 50. The
// recurrence is unchanged (see Part I's own doc comment): row i of the "arr1[i]
// == j" DP is a prefix sum of row i-1, shifted by delta = max(0, nums[i] -
// nums[i-1]).
//
// The bigger maxValue is why this problem exists as a separate LeetCode entry: the
// O(n * maxValue^2) brute-force row is still correct here, just no longer the
// arm you'd ship - O(n * maxValue) prefix sums is.
internal static class FindTheCountOfMonotonicPairsIISolution
{
    // Textbook baseline: re-sum each row's prefix from scratch per j. Correct at
    // any maxValue, but O(n * maxValue^2) - the arm the prefix-sum DP below has
    // to beat now that maxValue can reach 1000.
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

    // A running prefix sum turns each row into O(maxValue) instead of
    // O(maxValue^2) - O(n * maxValue) overall, which is what keeps this
    // tractable once nums[i] reaches 1000. Domain.Modular.ModularArithmetic
    // supplies the 1e9+7 both Monotonic Pairs problems report under.
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
