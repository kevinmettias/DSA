using MonotonicDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

// LeetCode 3956. Maximum Sum of M Non-Overlapping Subarrays I: choose between 1
// and maxSubarrays disjoint subarrays, each of length in [minLength, maxLength],
// maximizing their total sum. n <= 1000 here, so the textbook O(n*m*(r-l+1)) DP
// already fits; Part II (LC 3957) is the same rules at n <= 1e5, where that DP
// stops being fast enough and a fundamentally different technique takes over.
//
// Both strategies below solve the same recurrence: dpExact[i][j] = the best
// sum using EXACTLY j disjoint subarrays fully inside nums[0..i), or
// "infeasible" when fewer than j*minLength elements are available. Tracking the
// EXACT count (not "at most j") is what keeps "at least one subarray" correct - an
// "at most j" formulation would let j = 0 (sum 0) win whenever every subarray
// is a net loss, which LeetCode's own example 4 explicitly forbids. The answer
// is the best dpExact[elementCount][j] over j = 1..maxSubarrays.
internal static class MaximumSumOfMNonOverlappingSubarraysISolution
{
    private const long Infeasible = long.MinValue / 2;

    // Recomputes the inner max over every candidate length in [minLength, maxLength]
    // from scratch for each (i, j) - O(n*m*(r-l+1)), the arm the sliding-window
    // strategy below has to beat.
    public static long MaximumSumByDynamicProgramming(int[] nums, int maxSubarrays, int minLength, int maxLength)
    {
        var elementCount = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = BuildInfeasibleGrid(elementCount, maxSubarrays);

        FillByFreshScans(prefix, dp, (minLength, maxLength), maxSubarrays);

        return BestOverAtLeastOneSubarray(dp, elementCount, maxSubarrays);
    }

    // Fills the whole exact-count grid by recomputing the inner max over every candidate
    // length in [minLength, maxLength] from scratch for each (i, j) - O(n*m*(r-l+1)), the
    // arm the sliding-window strategy below has to beat.
    private static void FillByFreshScans(
        long[] prefix, long[][] dp, (int MinLength, int MaxLength) lengthBounds, int maxSubarrays)
    {
        var elementCount = prefix.Length - 1;

        for (var i = 1; i <= elementCount; i++)
        {
            for (var j = 1; j <= maxSubarrays; j++)
            {
                dp[i][j] = BestFreshScan(prefix, dp, lengthBounds, (i, j));
            }
        }
    }

    // The best total for exactly `Subarrays` disjoint subarrays ending at `End`: the best
    // of not taking another one at all and of every admissible final length in
    // [MinLength, MaxLength].
    private static long BestFreshScan(
        long[] prefix, long[][] dp, (int MinLength, int MaxLength) lengthBounds, (int End, int Subarrays) position)
    {
        var best = dp[position.End - 1][position.Subarrays];

        for (var length = lengthBounds.MinLength; length <= lengthBounds.MaxLength && length <= position.End; length++)
        {
            var start = position.End - length;

            if (dp[start][position.Subarrays - 1] == Infeasible)
            {
                continue;
            }

            best = Math.Max(best, dp[start][position.Subarrays - 1] + prefix[position.End] - prefix[start]);
        }

        return best;
    }

    // Same recurrence, but the inner max over length in [minLength, maxLength] is
    // answered from a monotonic deque of candidate starts i - length instead of a
    // fresh scan: as i advances by one, the window of valid starts [i-r, i-l] slides
    // by one, so tracking the best (dp[start][j-1] - prefix[start]) per j-layer is
    // O(n) per layer and O(n*m) overall - the same sliding-window-maximum shape
    // CountPartitionsWithMaxMinDifferenceAtMostKSolution already uses over
    // Deque<int>, here maximizing a DP transition instead of bounding a
    // max-min gap.
    public static long MaximumSumBySlidingWindowMaximum(int[] nums, int maxSubarrays, int minLength, int maxLength)
    {
        var elementCount = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = BuildInfeasibleGrid(elementCount, maxSubarrays);

        for (var j = 1; j <= maxSubarrays; j++)
        {
            SolveLayer((dp, prefix, j), (minLength, maxLength));
        }

        return BestOverAtLeastOneSubarray(dp, elementCount, maxSubarrays);
    }

    // A layer is one j of the recurrence together with what its transitions read: the
    // grid being filled, the prefix sums, and the layer index itself.
    private static void SolveLayer(
        (long[][] Dp, long[] Prefix, int J) layer, (int MinLength, int MaxLength) lengthBounds)
    {
        var (dp, prefix, j) = layer;
        var n = prefix.Length - 1;
        var deque = new MonotonicDeque();

        for (var i = 1; i <= n; i++)
        {
            dp[i][j] = BestAtPosition(layer, lengthBounds, deque, i);
        }
    }

    // One position of a layer: admit the start that has just entered the sliding window
    // [i - MaxLength, i - MinLength], drop the candidate starts that have left it, and
    // take the best transition still inside it.
    private static long BestAtPosition(
        (long[][] Dp, long[] Prefix, int J) layer, (int MinLength, int MaxLength) lengthBounds,
        MonotonicDeque deque, int position)
    {
        var (dp, prefix, j) = layer;
        var enter = position - lengthBounds.MinLength;

        if (enter >= 0 && dp[enter][j - 1] != Infeasible)
        {
            PushCandidate(deque, layer, enter);
        }

        while (deque.TryPeekFront(out var front) && front < position - lengthBounds.MaxLength)
        {
            deque.TryPopFront(out _);
        }

        var best = dp[position - 1][j];

        if (deque.TryPeekFront(out var start))
        {
            best = Math.Max(best, dp[start][j - 1] + prefix[position] - prefix[start]);
        }

        return best;
    }

    private static void PushCandidate(MonotonicDeque deque, (long[][] Dp, long[] Prefix, int J) layer, int start)
    {
        var (dp, prefix, j) = layer;
        var key = dp[start][j - 1] - prefix[start];

        while (deque.TryPeekBack(out var backStart) && dp[backStart][j - 1] - prefix[backStart] <= key)
        {
            deque.TryPopBack(out _);
        }

        deque.PushBack(start);
    }

    private static long BestOverAtLeastOneSubarray(long[][] dp, int elementCount, int maxSubarrays)
    {
        var best = Infeasible;

        for (var j = 1; j <= maxSubarrays; j++)
        {
            best = Math.Max(best, dp[elementCount][j]);
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
    private static long[][] BuildInfeasibleGrid(int elementCount, int maxSubarrays)
    {
        var dp = new long[elementCount + 1][];

        for (var i = 0; i <= elementCount; i++)
        {
            dp[i] = new long[maxSubarrays + 1];
            Array.Fill(dp[i], Infeasible);
            dp[i][0] = 0;
        }

        return dp;
    }
}
