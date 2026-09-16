using DSAExperimentation.Domain.Modular;
using MonotonicDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.CountPartitionsWithMaxMinDifferenceAtMostK;

// LeetCode 3578. Count Partitions With Max-Min Difference at Most K: split nums into
// contiguous, non-empty segments where every segment's max - min is at most k, and
// count the ways to do it, modulo 1e9+7.
//
// Both strategies solve the same recurrence - dp[r] = the number of ways to partition
// the first r elements, summing dp[l-1] over every l whose segment [l, r] is valid -
// and differ only in how they find, for each r, the smallest valid l. BruteForce
// rescans the window from scratch; SlidingWindowDeque tracks it with two monotonic
// Deque<int> index windows, the same primitive SlidingWindowMaximum (LC 239) already
// uses for a single running maximum, doubled up for a running minimum too.
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

            sum = (sum + dp[l - 1]) % ModularArithmetic.Modulo;
        }

        return sum;
    }

    // The same recurrence, but the smallest valid l is tracked with a two-pointer
    // sweep: as r grows, two monotonic index deques (one decreasing for the running
    // max, one increasing for the running min) answer "what is max - min over
    // [left, r-1]" in O(1), and left only ever moves forward, so the whole sweep is
    // O(n) amortized. A running prefix sum turns "sum of dp[l-1] over a range" into
    // one subtraction instead of a rescan.
    public static int CountPartitionsBySlidingWindowDeque(int[] nums, int maxDifference)
    {
        var n = nums.Length;
        var dp = new long[n + 1];
        var prefixSum = new long[n + 1];
        dp[0] = 1;
        prefixSum[0] = 1;

        var deques = (Max: new MonotonicDeque(), Min: new MonotonicDeque());
        var left = 0;

        for (var r = 1; r <= n; r++)
        {
            left = AdvanceLeft((nums, maxDifference), r - 1, left, deques);

            var lower = PrefixSumBefore(prefixSum, left);
            dp[r] = ((prefixSum[r - 1] - lower) % ModularArithmetic.Modulo + ModularArithmetic.Modulo)
                % ModularArithmetic.Modulo;
            prefixSum[r] = (prefixSum[r - 1] + dp[r]) % ModularArithmetic.Modulo;
        }

        return (int)dp[n];
    }

    // Brings the element at `index` into both monotonic deques, then advances left past
    // every window whose max - min still exceeds `maxDifference`, returning the advanced
    // left.
    private static int AdvanceLeft(
        (int[] Nums, int MaxDifference) problem, int index, int left, (MonotonicDeque Max, MonotonicDeque Min) deques)
    {
        PushMax(deques.Max, problem.Nums, index);
        PushMin(deques.Min, problem.Nums, index);

        return ShrinkToValidWindow(deques, problem.Nums, problem.MaxDifference, left);
    }

    private static void PushMax(MonotonicDeque maxDeque, int[] nums, int index)
    {
        while (maxDeque.TryPeekBack(out var backIndex) && nums[backIndex] <= nums[index])
        {
            maxDeque.TryPopBack(out _);
        }

        maxDeque.PushBack(index);
    }

    private static void PushMin(MonotonicDeque minDeque, int[] nums, int index)
    {
        while (minDeque.TryPeekBack(out var backIndex) && nums[backIndex] >= nums[index])
        {
            minDeque.TryPopBack(out _);
        }

        minDeque.PushBack(index);
    }

    // The running max and running min deques travel together everywhere in this file -
    // AdvanceLeft already hands them over as one `deques` - so the shrink takes that
    // same pair rather than splitting it back into two parameters.
    private static int ShrinkToValidWindow(
        (MonotonicDeque Max, MonotonicDeque Min) deques, int[] nums, int maxDifference, int left)
    {
        while (TryGetFrontSpan(deques.Max, deques.Min, out var frontMax, out var frontMin) &&
               nums[frontMax] - nums[frontMin] > maxDifference)
        {
            if (frontMax == left)
            {
                deques.Max.TryPopFront(out _);
            }

            if (frontMin == left)
            {
                deques.Min.TryPopFront(out _);
            }

            left++;
        }

        return left;
    }

    // The window's max and min are the two deques' fronts, so the shrink test needs
    // both of them readable before it can compare; the peeked values are only used
    // when this reports true.
    private static bool TryGetFrontSpan(
        MonotonicDeque maxDeque, MonotonicDeque minDeque, out int frontMax, out int frontMin)
    {
        frontMax = 0;
        frontMin = 0;

        return maxDeque.TryPeekFront(out frontMax) && minDeque.TryPeekFront(out frontMin);
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
