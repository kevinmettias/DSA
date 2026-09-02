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
    // max - min <= k, found by rescanning backward from r and shrinking on the
    // first violation - O(n) per r, O(n^2) overall.
    public static int CountPartitionsByBruteForce(int[] nums, int k)
    {
        var n = nums.Length;
        var dp = new long[n + 1];
        dp[0] = 1;

        for (var r = 1; r <= n; r++)
        {
            var windowMax = nums[r - 1];
            var windowMin = nums[r - 1];
            var sum = 0L;

            for (var l = r; l >= 1; l--)
            {
                windowMax = Math.Max(windowMax, nums[l - 1]);
                windowMin = Math.Min(windowMin, nums[l - 1]);

                if (windowMax - windowMin > k)
                {
                    break;
                }

                sum = (sum + dp[l - 1]) % ModularArithmetic.Modulo;
            }

            dp[r] = sum;
        }

        return (int)dp[n];
    }

    // The same recurrence, but the smallest valid l is tracked with a two-pointer
    // sweep: as r grows, two monotonic index deques (one decreasing for the running
    // max, one increasing for the running min) answer "what is max - min over
    // [left, r-1]" in O(1), and left only ever moves forward, so the whole sweep is
    // O(n) amortized. A running prefix sum turns "sum of dp[l-1] over a range" into
    // one subtraction instead of a rescan.
    public static int CountPartitionsBySlidingWindowDeque(int[] nums, int k)
    {
        var n = nums.Length;
        var dp = new long[n + 1];
        var prefixSum = new long[n + 1];
        dp[0] = 1;
        prefixSum[0] = 1;

        var maxDeque = new MonotonicDeque();
        var minDeque = new MonotonicDeque();
        var left = 0;

        for (var r = 1; r <= n; r++)
        {
            var i = r - 1;
            PushMax(maxDeque, nums, i);
            PushMin(minDeque, nums, i);
            left = ShrinkToValidWindow(maxDeque, minDeque, nums, k, left);

            var lower = left > 0 ? prefixSum[left - 1] : 0;
            dp[r] = ((prefixSum[r - 1] - lower) % ModularArithmetic.Modulo + ModularArithmetic.Modulo)
                % ModularArithmetic.Modulo;
            prefixSum[r] = (prefixSum[r - 1] + dp[r]) % ModularArithmetic.Modulo;
        }

        return (int)dp[n];
    }

    private static void PushMax(MonotonicDeque maxDeque, int[] nums, int i)
    {
        while (maxDeque.TryPeekBack(out var backIndex) && nums[backIndex] <= nums[i])
        {
            maxDeque.TryPopBack(out _);
        }

        maxDeque.PushBack(i);
    }

    private static void PushMin(MonotonicDeque minDeque, int[] nums, int i)
    {
        while (minDeque.TryPeekBack(out var backIndex) && nums[backIndex] >= nums[i])
        {
            minDeque.TryPopBack(out _);
        }

        minDeque.PushBack(i);
    }

    private static int ShrinkToValidWindow(MonotonicDeque maxDeque, MonotonicDeque minDeque, int[] nums, int k, int left)
    {
        while (maxDeque.TryPeekFront(out var frontMax) && minDeque.TryPeekFront(out var frontMin) &&
               nums[frontMax] - nums[frontMin] > k)
        {
            if (frontMax == left)
            {
                maxDeque.TryPopFront(out _);
            }

            if (frontMin == left)
            {
                minDeque.TryPopFront(out _);
            }

            left++;
        }

        return left;
    }
}
