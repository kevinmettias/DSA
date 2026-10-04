using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.LeetCode.HouseRobberII;

// LeetCode 213. House Robber II: same as #198's House Robber, except the houses
// form a circle, so the first and last house are adjacent too.
//
// A circular arrangement of n houses reduces to two runs of #198's own linear
// problem - houses [0, n-2] (excluding the last) and [1, n-1] (excluding the
// first) - because any valid selection must skip at least one of the two
// wrap-around neighbors, and the better of the two exclusions is always safe to
// take. Each run is #198's answer over a stretch of the street, so each arm here
// hands both runs to #198's matching arm (ARCHITECTURE 17.3): the rolling totals
// walk them directly, the memoized recursion hands each to this repo's Memoizer.
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

        var excludingLastHouse = HouseRobberSolution.RobByIterativeRollingTotals(nums, ..^1);
        var excludingFirstHouse = HouseRobberSolution.RobByIterativeRollingTotals(nums, 1..);

        return Math.Max(excludingLastHouse, excludingFirstHouse);
    }

    public static int RobByMemoizedRecursion(int[] nums)
    {
        if (nums.Length == 1)
        {
            return nums[0];
        }

        var excludingLastHouse = HouseRobberSolution.RobByMemoizedRecursion(nums, ..^1);
        var excludingFirstHouse = HouseRobberSolution.RobByMemoizedRecursion(nums, 1..);

        return Math.Max(excludingLastHouse, excludingFirstHouse);
    }
}
