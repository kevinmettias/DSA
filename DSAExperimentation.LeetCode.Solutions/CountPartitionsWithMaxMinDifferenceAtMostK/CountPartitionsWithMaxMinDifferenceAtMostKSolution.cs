using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.Solutions.CountPartitionsWithMaxMinDifferenceAtMostK;

// LeetCode 3578. Count Partitions With Max-Min Difference at Most K: split nums into
// contiguous, non-empty segments where every segment's max - min is at most k, and
// count the ways to do it, modulo 1e9+7.
//
// Both strategies solve the same recurrence - dp[r] = the number of ways to partition
// the first r elements, summing dp[l-1] over every l whose segment [l, r] is valid -
// and differ only in how they find, for each r, the smallest valid l. BruteForce
// rescans the window from scratch; SlidingWindowDeque tracks it with a MinMaxWindow,
// two of the MonotonicDeques SlidingWindowMaximum (LC 239) already uses for a single
// running maximum, one of them under MinWindowOrder for a running minimum too.
[LeetCodeQuestion("count-partitions-with-max-min-difference-at-most-k", LeetCodeAnswerOrder.AsWritten)]
internal static class CountPartitionsWithMaxMinDifferenceAtMostKSolution
{
    // dp[r] = sum of dp[l-1] over every l in [1, r] whose segment [l, r] has
    // max - min at most maxDifference, found by rescanning backward from r and
    // shrinking on the first violation - O(n) per r, O(n^2) overall.
    public static int CountPartitionsByBruteForce(int[] nums, int maxDifference)
    {
        var n = nums.Length;
        var dp = new long[n + 1];
        dp[0] = 1;

        for (var r = 1; r <= n; r++)
        {
            dp[r] = SumValidStarts(nums, maxDifference, r, dp);
        }

        return (int)dp[n];
    }

    // The sum of dp[l-1] over every l in [1, r] whose segment [l, r] has
    // max - min at most maxDifference, found by extending the window one element
    // at a time backwards from r and stopping at the first violation.
    private static long SumValidStarts(int[] nums, int maxDifference, int rightEnd, long[] dp)
    {
        var windowMax = nums[rightEnd - 1];
        var windowMin = nums[rightEnd - 1];
        var sum = 0L;

        for (var l = rightEnd; l >= 1; l--)
        {
            windowMax = Math.Max(windowMax, nums[l - 1]);
            windowMin = Math.Min(windowMin, nums[l - 1]);

            if (windowMax - windowMin > maxDifference)
            {
                break;
            }

            sum = (sum + dp[l - 1]) % LeetCodeModulus.Value;
        }

        return sum;
    }

    // The same recurrence, but the smallest valid l is tracked with a two-pointer
    // sweep: as r grows, a MinMaxWindow (a monotonic deque for the running max and one
    // for the running min) answers "what is max - min over [left, r-1]" in O(1), and
    // left only ever moves forward, so the whole sweep is O(n) amortized. A running
    // prefix sum turns "sum of dp[l-1] over a range" into one subtraction instead of a
    // rescan.
    public static int CountPartitionsBySlidingWindowDeque(int[] nums, int maxDifference)
    {
        var n = nums.Length;
        var dp = new long[n + 1];
        var prefixSum = new long[n + 1];
        dp[0] = 1;
        prefixSum[0] = 1;

        var window = new MinMaxWindow<int>();
        var left = 0;

        for (var r = 1; r <= n; r++)
        {
            left = AdvanceLeft((nums, maxDifference), r - 1, left, window);

            var lower = PrefixSumBefore(prefixSum, left);
            dp[r] = ((prefixSum[r - 1] - lower) % LeetCodeModulus.Value + LeetCodeModulus.Value)
                % LeetCodeModulus.Value;
            prefixSum[r] = (prefixSum[r - 1] + dp[r]) % LeetCodeModulus.Value;
        }

        return (int)dp[n];
    }

    // Brings the element at `index` into the window, then advances left past every
    // window whose max - min still exceeds `maxDifference`, returning the advanced left.
    private static int AdvanceLeft(
        (int[] Nums, int MaxDifference) problem,
        int index,
        int left,
        MinMaxWindow<int> window)
    {
        window.Push(index, problem.Nums[index]);

        return ShrinkToValidWindow(window, problem.MaxDifference, left);
    }

    // Each step past the left edge drops whatever entries it leaves behind.
    private static int ShrinkToValidWindow(MinMaxWindow<int> window, int maxDifference, int left)
    {
        while (window.TryPeekRange(out var minimum, out var maximum) && maximum - minimum > maxDifference)
        {
            left++;
            window.EvictBefore(left);
        }

        return left;
    }

    // The running prefix sum strictly before index left, or 0 at the very start of the
    // array, where a window beginning at 0 has no preceding prefix to subtract.
    private static long PrefixSumBefore(long[] prefixSum, int left)
    {
        if (left == 0)
        {
            return 0;
        }

        return prefixSum[left - 1];
    }
}
