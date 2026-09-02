using MonotonicDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

// LeetCode 3956. Maximum Sum of M Non-Overlapping Subarrays I: choose between 1
// and m disjoint subarrays, each of length in [l, r], maximizing their total
// sum. n <= 1000 here, so the textbook O(n*m*(r-l+1)) DP already fits; Part II
// (LC 3957) is the same rules at n <= 1e5, where that DP stops being fast
// enough and a fundamentally different technique takes over.
//
// Both strategies below solve the same recurrence: dpExact[i][j] = the best
// sum using EXACTLY j disjoint subarrays fully inside nums[0..i), or
// "infeasible" when fewer than j*l elements are available. Tracking the EXACT
// count (not "at most j") is what keeps "at least one subarray" correct - an
// "at most j" formulation would let j = 0 (sum 0) win whenever every subarray
// is a net loss, which LeetCode's own example 4 explicitly forbids. The answer
// is the best dpExact[n][j] over j = 1..m.
internal static class MaximumSumOfMNonOverlappingSubarraysISolution
{
    private const long Infeasible = long.MinValue / 2;

    // Recomputes the inner max over every candidate length in [l, r] from
    // scratch for each (i, j) - O(n*m*(r-l+1)), the arm the sliding-window
    // strategy below has to beat.
    public static long MaximumSumByDynamicProgramming(int[] nums, int m, int l, int r)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = BuildInfeasibleGrid(n, m);

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var best = dp[i - 1][j];

                for (var length = l; length <= r && length <= i; length++)
                {
                    var start = i - length;

                    if (dp[start][j - 1] == Infeasible)
                    {
                        continue;
                    }

                    best = Math.Max(best, dp[start][j - 1] + prefix[i] - prefix[start]);
                }

                dp[i][j] = best;
            }
        }

        return BestOverAtLeastOneSubarray(dp, n, m);
    }

    // Same recurrence, but the inner max over length in [l, r] is answered from
    // a monotonic deque of candidate starts i - length instead of a fresh scan:
    // as i advances by one, the window of valid starts [i-r, i-l] slides by
    // one, so tracking the best (dp[start][j-1] - prefix[start]) per j-layer is
    // O(n) per layer and O(n*m) overall - the same sliding-window-maximum shape
    // CountPartitionsWithMaxMinDifferenceAtMostKSolution already uses over
    // Deque<int>, here maximizing a DP transition instead of bounding a
    // max-min gap.
    public static long MaximumSumBySlidingWindowMaximum(int[] nums, int m, int l, int r)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = BuildInfeasibleGrid(n, m);

        for (var j = 1; j <= m; j++)
        {
            SolveLayer(dp, prefix, n, l, r, j);
        }

        return BestOverAtLeastOneSubarray(dp, n, m);
    }

    private static void SolveLayer(long[][] dp, long[] prefix, int n, int l, int r, int j)
    {
        var deque = new MonotonicDeque();

        for (var i = 1; i <= n; i++)
        {
            var enter = i - l;

            if (enter >= 0 && dp[enter][j - 1] != Infeasible)
            {
                PushCandidate(deque, dp, prefix, j, enter);
            }

            while (deque.TryPeekFront(out var front) && front < i - r)
            {
                deque.TryPopFront(out _);
            }

            var best = dp[i - 1][j];

            if (deque.TryPeekFront(out var start))
            {
                best = Math.Max(best, dp[start][j - 1] + prefix[i] - prefix[start]);
            }

            dp[i][j] = best;
        }
    }

    private static void PushCandidate(MonotonicDeque deque, long[][] dp, long[] prefix, int j, int start)
    {
        var key = dp[start][j - 1] - prefix[start];

        while (deque.TryPeekBack(out var backStart) && dp[backStart][j - 1] - prefix[backStart] <= key)
        {
            deque.TryPopBack(out _);
        }

        deque.PushBack(start);
    }

    private static long BestOverAtLeastOneSubarray(long[][] dp, int n, int m)
    {
        var best = Infeasible;

        for (var j = 1; j <= m; j++)
        {
            best = Math.Max(best, dp[n][j]);
        }

        return best;
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        return prefix;
    }

    // dp[i][0] = 0 for every prefix length i (zero subarrays always costs
    // nothing); every other cell starts Infeasible until a transition proves
    // otherwise.
    private static long[][] BuildInfeasibleGrid(int n, int m)
    {
        var dp = new long[n + 1][];

        for (var i = 0; i <= n; i++)
        {
            dp[i] = new long[m + 1];
            Array.Fill(dp[i], Infeasible);
            dp[i][0] = 0;
        }

        return dp;
    }
}
