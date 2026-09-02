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
        Memoizer.Memoize<int, int>(0, (i, rob) =>
            i >= nums.Length ? 0 : Math.Max(rob(i + 1), nums[i] + rob(i + 2)));
}
