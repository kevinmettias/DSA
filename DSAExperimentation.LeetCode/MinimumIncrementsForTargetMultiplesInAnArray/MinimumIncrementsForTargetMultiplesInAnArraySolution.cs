using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumIncrementsForTargetMultiplesInAnArray;

// LC 3444: increment elements of nums (by 1 each, any number of times) so that
// every target[j] divides at least one element of nums, minimizing total
// increments. target.Length <= 4, so which subset of targets a single nums[i] ends
// up covering (by being raised to a multiple of that subset's LCM) is a bitmask
// over at most 16 states - a textbook subset-cover DP over "targets covered so
// far", assigning one nums element at a time.
internal static class MinimumIncrementsForTargetMultiplesInAnArraySolution
{
    // The textbook bottom-up arm: a plain long[] indexed by bitmask, walked once per
    // nums element. Deliberately no repo primitive here - it is what the memoized
    // arm below has to justify itself against.
    public static long MinIncrementsByBottomUpBitmaskDp(int[] nums, int[] target) =>
        MinIncrementsByBottomUpBitmaskDp(nums, TargetLcmTable.Build(target));

    public static long MinIncrementsByBottomUpBitmaskDp(int[] nums, TargetLcmTable lcmTable)
    {
        var stateCount = 1 << lcmTable.TargetCount;
        var dp = new long[stateCount];
        Array.Fill(dp, long.MaxValue);
        dp[0] = 0;

        foreach (var num in nums)
        {
            dp = RelaxOneElement(dp, num, lcmTable, stateCount);
        }

        return dp[stateCount - 1];
    }

    // One nums element's pass: next[] starts as a copy of dp (this element skipped, so
    // every state it already reached survives untouched) and each settled coverage state
    // relaxes into the states reachable by raising this element to some subset's LCM.
    private static long[] RelaxOneElement(long[] dp, int num, TargetLcmTable lcmTable, int stateCount)
    {
        var next = (long[])dp.Clone();

        for (var covered = 0; covered < stateCount; covered++)
        {
            if (dp[covered] == long.MaxValue)
            {
                continue;
            }

            for (var subset = 1; subset < stateCount; subset++)
            {
                var candidate = dp[covered] + IncrementCost(num, lcmTable.SubsetLcm[subset]);
                var reached = covered | subset;

                if (candidate < next[reached])
                {
                    next[reached] = candidate;
                }
            }
        }

        return next;
    }

    // Same recurrence, reduced to this repo's own Memoizer: the state is "how many
    // nums are left to decide, which targets are still uncovered", and the choice at
    // each nums element is either skip it or raise it to some subset's LCM.
    public static long MinIncrementsByMemoizedBitmaskDp(int[] nums, int[] target) =>
        MinIncrementsByMemoizedBitmaskDp(nums, TargetLcmTable.Build(target));

    public static long MinIncrementsByMemoizedBitmaskDp(int[] nums, TargetLcmTable lcmTable)
    {
        var fullMask = (1 << lcmTable.TargetCount) - 1;

        return Memoizer.Memoize((0, 0), new FewestIncrementsFrom(nums, lcmTable, fullMask));
    }

    // The recurrence, as a named type: every target is covered once `state.Covered`
    // reaches the full mask, and until then each nums element is either skipped or
    // raised to one subset's LCM, finishing the rest from whatever that leaves.
    private sealed class FewestIncrementsFrom(int[] nums, TargetLcmTable lcmTable, int fullMask)
        : IRecurrence<(int Index, int Covered), long>
    {
        public long Replay((int Index, int Covered) state, IRecurrence<(int Index, int Covered), long> rest)
        {
            if (state.Covered == fullMask)
            {
                return 0;
            }

            return BestCostFrom(state, rest);
        }

        // The cost of covering whatever targets are still uncovered from `state`: running
        // out of nums first is unreachable (the bottom-up arm's long.MaxValue, halved so
        // the additions below cannot overflow), and otherwise this element is either
        // skipped or raised to one subset's LCM, whichever is cheaper.
        private long BestCostFrom((int Index, int Covered) state, IRecurrence<(int Index, int Covered), long> rest)
        {
            if (state.Index == nums.Length)
            {
                return long.MaxValue / 2;
            }

            var best = rest.Replay((state.Index + 1, state.Covered), rest);

            for (var subset = 1; subset <= fullMask; subset++)
            {
                var cost = IncrementCost(nums[state.Index], lcmTable.SubsetLcm[subset]);
                var candidate = cost + rest.Replay((state.Index + 1, state.Covered | subset), rest);

                if (candidate < best)
                {
                    best = candidate;
                }
            }

            return best;
        }
    }

    // How many +1 increments raise num to the next multiple of lcm (0 if it already is one).
    // The outer `% lcm` is what folds the already-a-multiple case into the same expression:
    // a remainder of 0 gives back lcm - 0, which the modulo returns as 0.
    private static long IncrementCost(long num, long lcm) => (lcm - (num % lcm)) % lcm;
}
