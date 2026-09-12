using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.TargetSum;

// LeetCode 494. Target Sum: count the +/- sign assignments over nums that sum to
// target.
//
// The naive strategy is a plain top-down +/- recursion over (index, runningSum) -
// exponential, since the same (index, runningSum) pair recurs through many different
// sign-choice paths. The composed strategy memoizes that exact recursion through this
// repo's own Memoizer<TState, TResult> (DecodeWays/PredictTheWinner precedent), so
// each (index, runningSum) state is only ever solved once.
internal static class TargetSumSolution
{
    // Deliberately written without this repo's primitives - it is the arm the
    // memoized strategy has to justify itself against. N must stay modest: the
    // recursion is 2^N.
    public static int WaysByUnmemoizedRecursion(int[] nums, int target) => CountWays(nums, target, 0, 0);

    private static int CountWays(int[] nums, int target, int index, int sum)
    {
        if (index == nums.Length)
        {
            return sum == target ? 1 : 0;
        }

        return CountWays(nums, target, index + 1, sum + nums[index])
             + CountWays(nums, target, index + 1, sum - nums[index]);
    }

    public static int WaysByMemoizedRecursion(int[] nums, int target)
    {
        return Memoizer.Memoize<(int Index, int Sum), int>((0, 0), CountWaysMemoized);

        int CountWaysMemoized((int Index, int Sum) state, Func<(int Index, int Sum), int> ways)
        {
            if (state.Index == nums.Length)
            {
                return state.Sum == target ? 1 : 0;
            }

            return ways((state.Index + 1, state.Sum + nums[state.Index]))
                 + ways((state.Index + 1, state.Sum - nums[state.Index]));
        }
    }
}
