using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumTimeToMakeArraySumAtMostX;

// LeetCode 2809. Minimum Time to Make Array Sum At Most x: every second, all of
// nums1 grows by its nums2 counterpart, and then one index may be zeroed. Return
// the fewest seconds after which sum(nums1) <= x, or -1 if no schedule reaches it.
//
// The exchange argument both strategies rest on: zeroing index i in time slot j is
// worth nums1[i] + nums2[i]*j, so an optimal schedule always assigns the smallest
// nums2 to the earliest slots. Sort the (nums1, nums2) pairs ascending by nums2
// and the problem becomes a 0/1 knapsack over "how many indices have been zeroed
// so far", where the count zeroed doubles as the time slot the next one gets. With
// reduction[t] the best total saving from zeroing exactly t of the sorted indices,
// sum1 + t*sum2 - reduction[t] is nums1's total after t seconds, so the answer is
// the first t at which it drops to x.
//
// Both strategies run that same O(n^2) recurrence; they differ only in the DP's
// footprint - a dense table with a row per prefix, or the standard backward
// iteration that collapses the table onto one rolling row.
internal static class MinimumTimeToMakeArraySumAtMostXSolution
{
    // The textbook answer: Array.Sort with a comparison delegate and a dense
    // long[n+1, n+1] table, dp[i, j] being the best reduction from the first i
    // sorted pairs with exactly j of them zeroed - O(n^2) extra space.
    // Deliberately written with nothing but BCL arrays; it is the arm the rolling
    // row below has to justify itself against, and running the identical
    // recurrence makes it a correctness cross-check rather than a second
    // algorithm.
    public static int MinimumTimeByDenseTable(int[] nums1, int[] nums2, int x)
    {
        var n = nums1.Length;
        var pairs = Pairs(nums1, nums2);
        Array.Sort(pairs, (a, b) => a.Nums2.CompareTo(b.Nums2));

        var dp = new long[n + 1, n + 1];

        for (var i = 1; i <= n; i++)
        {
            FillRow(dp, pairs[i - 1], i, n);
        }

        return FirstSecondAtOrBelow(nums1, nums2, x, t => dp[n, t]);
    }

    // Row i of the dense table: either the pair is left alone, keeping row i-1's
    // value, or it is zeroed in slot j, taking row i-1's j-1 entry plus what
    // zeroing it in that slot is worth.
    private static void FillRow(long[,] dp, (int Nums1, int Nums2) pair, int i, int n)
    {
        var (a1, a2) = pair;

        for (var j = 0; j <= n; j++)
        {
            dp[i, j] = dp[i - 1, j];

            if (j >= 1)
            {
                dp[i, j] = Math.Max(dp[i, j], dp[i - 1, j - 1] + a1 + ((long)a2 * j));
            }
        }
    }

    // The same recurrence over this repo's own MergeSort - applied to an
    // ArrayIndexedSequence<(int, int)>, the tuple-sort idiom MaximumSumQueries
    // uses - with the table collapsed to a single long[n+1] rolling row by
    // iterating j downwards, so each pair is still used at most once. O(n) extra
    // space instead of O(n^2).
    public static int MinimumTimeByRollingKnapsack(int[] nums1, int[] nums2, int x)
    {
        var n = nums1.Length;
        var pairs = Pairs(nums1, nums2);
        SortAscendingByNums2(pairs);

        var dp = new long[n + 1];

        for (var i = 0; i < n; i++)
        {
            var (a1, a2) = pairs[i];

            for (var j = Math.Min(i + 1, n); j >= 1; j--)
            {
                dp[j] = Math.Max(dp[j], dp[j - 1] + a1 + ((long)a2 * j));
            }
        }

        return FirstSecondAtOrBelow(nums1, nums2, x, t => dp[t]);
    }

    private static void SortAscendingByNums2((int Nums1, int Nums2)[] pairs)
    {
        var byNums2Ascending = Comparer<(int Nums1, int Nums2)>.Create((a, b) => a.Nums2.CompareTo(b.Nums2));

        MergeSort.Sort<(int Nums1, int Nums2), ArrayIndexedSequence<(int Nums1, int Nums2)>>(
            new ArrayIndexedSequence<(int Nums1, int Nums2)>(pairs), byNums2Ascending);
    }

    private static (int Nums1, int Nums2)[] Pairs(int[] nums1, int[] nums2)
    {
        var pairs = new (int Nums1, int Nums2)[nums1.Length];

        for (var i = 0; i < nums1.Length; i++)
        {
            pairs[i] = (nums1[i], nums2[i]);
        }

        return pairs;
    }

    // After t seconds the untouched entries have each grown t times, so the total
    // is sum1 + t*sum2 less whatever the best t zeroings saved.
    private static int FirstSecondAtOrBelow(int[] nums1, int[] nums2, int x, Func<int, long> reductionAt)
    {
        var sum1 = Total(nums1);
        var sum2 = Total(nums2);

        for (var t = 0; t <= nums1.Length; t++)
        {
            if (sum1 + (sum2 * t) - reductionAt(t) <= x)
            {
                return t;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static long Total(int[] values)
    {
        long total = 0;

        foreach (var value in values)
        {
            total += value;
        }

        return total;
    }
}
