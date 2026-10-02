using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.HouseRobberII;

// LeetCode 213. House Robber II: same as #198's House Robber, except the houses
// form a circle, so the first and last house are adjacent too.
//
// A circular arrangement of n houses reduces to two runs of #198's own linear
// problem - houses [0, n-2] (excluding the last) and [1, n-1] (excluding the
// first) - because any valid selection must skip at least one of the two
// wrap-around neighbors, and the better of the two exclusions is always safe to
// take. Each run is #198's own linear rule, just started at a different index and
// capped at a different end: the memoized arm hands each run to this repo's
// Memoizer, the rolling-totals arm walks it directly.
internal static class HouseRobberIISolution
{
    // The textbook arm the memoized two-run split is measured against: each half of
    // the circle is walked with #198's skip-or-rob rule carried in two rolling
    // totals, so no Memoizer dictionary is built for either run and the circular
    // case costs O(1) space per run like the linear one.
    public static int RobByIterativeTwoPass(int[] nums)
    {
        if (nums.Length == 1)
        {
            return nums[0];
        }

        var excludingLastHouse = RobLinearRange(nums, 0, nums.Length - 2);
        var excludingFirstHouse = RobLinearRange(nums, 1, nums.Length - 1);
        return Math.Max(excludingLastHouse, excludingFirstHouse);
    }

    private static int RobLinearRange(int[] nums, int start, int end)
    {
        var bestBeforePrevious = 0;
        var bestPrevious = 0;

        for (var house = start; house <= end; house++)
        {
            var robThisHouse = bestBeforePrevious + nums[house];
            bestBeforePrevious = bestPrevious;
            bestPrevious = Math.Max(bestPrevious, robThisHouse);
        }

        return bestPrevious;
    }

    public static int RobByMemoizedRecursion(int[] nums)
    {
        if (nums.Length == 1)
        {
            return nums[0];
        }

        var excludingLastHouse = RobRange(nums, 0, nums.Length - 2);
        var excludingFirstHouse = RobRange(nums, 1, nums.Length - 1);
        return Math.Max(excludingLastHouse, excludingFirstHouse);
    }

    private static int RobRange(int[] nums, int start, int end) =>
        Memoizer.Memoize<int, int>(start, new BestRobberyThroughRun(nums, end));

    // The linear #198 rule, named: the best run through house `state` is the larger of
    // skipping that house and robbing it on top of the best run ending two houses
    // earlier. `end` caps the run, so the same rule serves both halves of the circle.
    private sealed class BestRobberyThroughRun(int[] nums, int end) : IRecurrence<int, int>
    {
        /// <inheritdoc/>
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state > end)
            {
                return 0;
            }

            var skip = rest.Replay(state + 1, rest);
            var take = nums[state] + rest.Replay(state + 2, rest);

            return Math.Max(skip, take);
        }
    }
}
