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

            dp = next;
        }

        return dp[stateCount - 1];
    }

    // Same recurrence, reduced to this repo's own Memoizer: the state is "how many
    // nums are left to decide, which targets are still uncovered", and the choice at
    // each nums element is either skip it or raise it to some subset's LCM.
    public static long MinIncrementsByMemoizedBitmaskDp(int[] nums, int[] target) =>
        MinIncrementsByMemoizedBitmaskDp(nums, TargetLcmTable.Build(target));

    public static long MinIncrementsByMemoizedBitmaskDp(int[] nums, TargetLcmTable lcmTable)
    {
        var fullMask = (1 << lcmTable.TargetCount) - 1;

        long Recurrence((int Index, int Covered) state, Func<(int, int), long> recurse)
        {
            if (state.Covered == fullMask)
            {
                return 0;
            }

            if (state.Index == nums.Length)
            {
                return long.MaxValue / 2;
            }

            var best = recurse((state.Index + 1, state.Covered));

            for (var subset = 1; subset <= fullMask; subset++)
            {
                var cost = IncrementCost(nums[state.Index], lcmTable.SubsetLcm[subset]);
                var candidate = cost + recurse((state.Index + 1, state.Covered | subset));

                if (candidate < best)
                {
                    best = candidate;
                }
            }

            return best;
        }

        return Memoizer.Memoize<(int Index, int Covered), long>((0, 0), Recurrence);
    }

    // How many +1 increments raise num to the next multiple of lcm (0 if it already is one).
    private static long IncrementCost(long num, long lcm)
    {
        var remainder = num % lcm;
        return remainder == 0 ? 0 : lcm - remainder;
    }
}
