using MonotonicDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysII;

// LeetCode 3957. Maximum Sum of M Non-Overlapping Subarrays II: the same rules
// as Part I (LC 3956) - between 1 and m disjoint subarrays, each of length in
// [l, r], maximum total sum - but n reaches 1e5 and m can reach n, where Part
// I's O(n*m) sliding-window DP is no longer fast enough.
//
// f(j) = the best sum using EXACTLY j disjoint subarrays is concave in j (the
// standard exchange-argument result for "choose up to k disjoint weighted
// intervals": swapping which j+1 intervals are kept can only ever gain less
// than the previous swap did), so the answer - max over j = 1..m of f(j) - is
// found by Lagrangian relaxation ("aliens trick"): binary search a
// per-subarray penalty lambda so that the unconstrained optimum (any number of
// subarrays, `lambda` charged against each) lands on a count <= m, then
// recover f(m) from that penalized optimum as value + lambda*m. Every
// candidate lambda costs one O(n) DP - Part I's SlidingWindowMaximum
// recurrence collapsed to a single unconstrained layer (no j dimension - the
// whole point of paying a penalty instead of tracking count directly), with a
// count carried alongside each value both to steer the search and to keep the
// "must select at least one" requirement from ever picking the free "select
// nothing" scaffolding every transition still needs available to draw from.
internal static class MaximumSumOfMNonOverlappingSubarraysIISolution
{
    private const long Infeasible = long.MinValue / 2;

    // nums[i] in [-1e5, 1e5] and a subarray spans at most n <= 1e5 elements, so
    // no marginal value (the sum of one candidate subarray) can exceed 1e10 in
    // magnitude - this is comfortably past that, the range LagrangianRelaxation
    // binary searches lambda over.
    private const long PenaltyBound = 50_000_000_000L;

    // Part I's textbook DP, unoptimized - correct at any n but only practical
    // well below LC's own n <= 1e5 ceiling. The arm LagrangianRelaxation has to
    // beat; kept here rather than shared with Part I's solution class because
    // every LeetCode problem folder answers its own LeetCode problem id
    // independently (see OpenTheLock, CountNumberOfTrapezoidsI/II).
    public static long MaximumSumByDynamicProgramming(int[] nums, int m, int l, int r)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = BuildInfeasibleGrid(n, m);

        for (var i = 1; i <= n; i++)
        {
            FillRowForEachCount(dp, prefix, i, (m, l, r));
        }

        return BestOverAtLeastOneSubarray(dp, n, m);
    }

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

    // Fills row i of Part I's table: dp[i][j] is the best sum using exactly j
    // disjoint subarrays fully inside nums[0..i), each of length in [l, r].
    private static void FillRowForEachCount(
        long[][] dp, long[] prefix, int i, (int MaxCount, int MinLength, int MaxLength) limits)
    {
        for (var j = 1; j <= limits.MaxCount; j++)
        {
            var best = dp[i - 1][j];

            for (var length = limits.MinLength; length <= limits.MaxLength && length <= i; length++)
            {
                var start = i - length;

                if (dp[start][j - 1] != Infeasible)
                {
                    best = Math.Max(best, dp[start][j - 1] + prefix[i] - prefix[start]);
                }
            }

            dp[i][j] = best;
        }
    }

    // The answer is the best "exactly j subarrays" entry across the whole j = 1..m
    // range, since using fewer than m subarrays is always allowed.
    private static long BestOverAtLeastOneSubarray(long[][] dp, int n, int m)
    {
        var answer = Infeasible;

        for (var j = 1; j <= m; j++)
        {
            answer = Math.Max(answer, dp[n][j]);
        }

        return answer;
    }

    // Binary searches the smallest non-negative penalty lambda whose penalized
    // optimum uses at most m subarrays, then reads f(m) off it: g(lambda) + lambda*m,
    // where g(lambda) is the penalized optimum's value. That formula is valid
    // at any lambda for which m itself is among the tied-optimal counts, and
    // the smallest lambda clearing the "<= m" binary search always lands in
    // that set - see PenalizedOptimum's own comment for the tie-break this
    // depends on.
    public static long MaximumSumByLagrangianRelaxation(int[] nums, int m, int l, int r)
    {
        var lowLambda = 0L;
        var highLambda = PenaltyBound;

        while (lowLambda < highLambda)
        {
            var midLambda = lowLambda + ((highLambda - lowLambda) / 2);
            var (_, count) = PenalizedOptimum(nums, l, r, midLambda);

            if (count <= m)
            {
                highLambda = midLambda;
            }
            else
            {
                lowLambda = midLambda + 1;
            }
        }

        var (value, _) = PenalizedOptimum(nums, l, r, lowLambda);

        return value + (lowLambda * m);
    }

    // best[i]: the unconstrained (any count, including zero) best (value, count)
    // using nums[0..i) with every subarray charged `lambda`. bestNonEmpty[i] is
    // the same restricted to count >= 1 - required so the final answer never
    // reports the trivial "pick nothing" state every transition still needs as
    // its own source (the segment before a newly placed subarray is always
    // free to be empty). Both share one monotonic deque over `best`.
    //
    // Ties are broken toward FEWER subarrays: this is what makes the count
    // returned here non-increasing as lambda grows, the property
    // LagrangianRelaxation's binary search depends on. The recovered VALUE at
    // the search's boundary lambda is correct regardless of which tied count
    // wins a given tie - only the count needs a consistent rule to keep the
    // search monotonic.
    private static (long Value, long Count) PenalizedOptimum(int[] nums, int l, int r, long lambda)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var best = new (long Value, long Count)[n + 1];
        var bestNonEmpty = new (long Value, long Count)[n + 1];
        Array.Fill(bestNonEmpty, (Infeasible, long.MaxValue));
        best[0] = (0, 0);

        var deque = new MonotonicDeque();
        var tables = (Best: best, NonEmpty: bestNonEmpty);

        for (var i = 1; i <= n; i++)
        {
            RefreshPenaltyWindow(prefix, best, deque, (i, l, r));
            AdvancePenalizedTables(prefix, tables, deque, (i, lambda));
        }

        return bestNonEmpty[n];
    }

    // Slide the window of starts a subarray ending at window.Index may use: i - l
    // just became eligible, and anything before i - r has expired.
    private static void RefreshPenaltyWindow(
        long[] prefix,
        (long Value, long Count)[] best,
        MonotonicDeque deque,
        (int Index, int MinLength, int MaxLength) window)
    {
        var enter = window.Index - window.MinLength;

        if (enter >= 0)
        {
            PushCandidate(deque, best, prefix, enter);
        }

        while (deque.TryPeekFront(out var front) && front < window.Index - window.MaxLength)
        {
            deque.TryPopFront(out _);
        }
    }

    // Maintains a deque of candidate starts, decreasing by best[start].Value -
    // prefix[start], with ties broken so the front always holds the fewest-
    // subarray candidate among equal keys - the same tie-break PreferFewerOnTie
    // applies at the transition itself.
    private static void PushCandidate(MonotonicDeque deque, (long Value, long Count)[] best, long[] prefix, int start)
    {
        var key = best[start].Value - prefix[start];
        var count = best[start].Count;

        while (deque.TryPeekBack(out var backStart))
        {
            var backKey = best[backStart].Value - prefix[backStart];

            if (IsDominatedByArriving(backKey, best[backStart].Count, key, count))
            {
                deque.TryPopBack(out _);
            }
            else
            {
                break;
            }
        }

        deque.PushBack(start);
    }

    // A queued candidate keeps its place only while it beats the arriving one: a smaller
    // key loses outright, and an equal key loses by carrying no fewer subarrays.
    private static bool IsDominatedByArriving(long backKey, long backCount, long key, long count) =>
        backKey < key || (backKey == key && backCount >= count);

    // Take the best transition off the window's front and let both tables carry the
    // better of their previous entry and it.
    private static void AdvancePenalizedTables(
        long[] prefix,
        ((long Value, long Count)[] Best, (long Value, long Count)[] NonEmpty) tables,
        MonotonicDeque deque,
        (int Index, long Lambda) at)
    {
        var (best, bestNonEmpty) = tables;

        (long Value, long Count)? transition = null;

        if (deque.TryPeekFront(out var start))
        {
            var value = best[start].Value - at.Lambda + prefix[at.Index] - prefix[start];
            transition = (value, best[start].Count + 1);
        }

        best[at.Index] = PreferFewerOnTie(best[at.Index - 1], transition);
        bestNonEmpty[at.Index] = PreferFewerOnTie(bestNonEmpty[at.Index - 1], transition);
    }

    private static (long Value, long Count) PreferFewerOnTie(
        (long Value, long Count) current, (long Value, long Count)? candidate)
    {
        if (candidate is not { } value)
        {
            return current;
        }

        var currentWins = current.Value > value.Value ||
            (current.Value == value.Value && current.Count <= value.Count);

        return currentWins ? current : value;
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
}
