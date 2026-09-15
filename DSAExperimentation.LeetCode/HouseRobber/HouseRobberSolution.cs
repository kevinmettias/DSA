using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.HouseRobber;

// LeetCode 198. House Robber: the max sum of non-adjacent house values along a
// street of houses.
//
// The state is just "starting from house i" - whichever houses upstream were
// robbed never needs tracking explicitly, because each state's own memoized
// recursion already accounts for both choices at i: skip it, or rob it and
// jump to i + 2.
internal static class HouseRobberSolution
{
    public static int RobByMemoizedRecursion(int[] nums) =>
        Memoizer.Memoize<int, int>(0, new BestHaulFromHouse(nums));

    /// <summary>
    /// The recurrence, named: from house <paramref name="state"/> the best haul is
    /// either the best haul from the next house, or this house's value plus the best
    /// haul from the house after it. Those two choices at every house are the whole
    /// of the rule - the decision the bare lambda left anonymous.
    /// </summary>
    private sealed class BestHaulFromHouse(int[] nums) : IRecurrence<int, int>
    {
        /// <inheritdoc/>
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state >= nums.Length)
            {
                return 0;
            }

            var skipThisHouse = rest.Replay(state + 1, rest);
            var robThisHouse = nums[state] + rest.Replay(state + 2, rest);

            return Math.Max(skipThisHouse, robThisHouse);
        }
    }
}
