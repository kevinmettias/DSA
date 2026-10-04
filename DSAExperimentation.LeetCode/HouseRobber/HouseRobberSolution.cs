using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.HouseRobber;

// LeetCode 198. House Robber: the max sum of non-adjacent house values along a
// street of houses.
//
// The state is just "starting from house i" - whichever houses upstream were
// robbed never needs tracking explicitly, because each state's own memoized
// recursion already accounts for both choices at i: skip it, or rob it and
// jump to i + 2.
//
// Each arm also robs any contiguous run of the street, given as a Range: LC 213
// (House Robber II) runs this problem's rule over two runs of its circle
// (ARCHITECTURE 17.3), and the whole street is just the run Range.All.
internal static class HouseRobberSolution
{
    // The textbook arm the memoized recurrence is measured against: the same
    // skip-or-rob rule rolled forward over the street, carrying only the two best
    // hauls the next house could build on. It pays neither a Memoizer dictionary
    // probe per house nor a call stack as deep as the street.
    public static int RobByIterativeRollingTotals(int[] nums) => RobByIterativeRollingTotals(nums, Range.All);

    public static int RobByIterativeRollingTotals(int[] nums, Range houses)
    {
        var (first, count) = houses.GetOffsetAndLength(nums.Length);
        var bestBeforePrevious = 0;
        var bestPrevious = 0;

        for (var house = first; house < first + count; house++)
        {
            var robThisHouse = bestBeforePrevious + nums[house];
            bestBeforePrevious = bestPrevious;
            bestPrevious = Math.Max(bestPrevious, robThisHouse);
        }

        return bestPrevious;
    }

    public static int RobByMemoizedRecursion(int[] nums) => RobByMemoizedRecursion(nums, Range.All);

    public static int RobByMemoizedRecursion(int[] nums, Range houses)
    {
        var (first, count) = houses.GetOffsetAndLength(nums.Length);

        return Memoizer.Memoize<int, int>(first, new BestHaulFromHouse(nums, first + count));
    }

    /// <summary>
    /// The recurrence, named: from house <paramref name="state"/> the best haul is
    /// either the best haul from the next house, or this house's value plus the best
    /// haul from the house after it. Those two choices at every house are the whole
    /// of the rule - the decision the bare lambda left anonymous. The run stops before
    /// <c>end</c>, so the same rule serves the whole street or any stretch of it.
    /// </summary>
    private sealed class BestHaulFromHouse(int[] nums, int end) : IRecurrence<int, int>
    {
        /// <inheritdoc/>
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state >= end)
            {
                return 0;
            }

            var skipThisHouse = rest.Replay(state + 1, rest);
            var robThisHouse = nums[state] + rest.Replay(state + 2, rest);

            return Math.Max(skipThisHouse, robThisHouse);
        }
    }
}
