using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TargetSum;

// LeetCode 494. Target Sum: a top-down +/- recursion over (index, runningSum) memoized
// through this repo's own Memoizer<TState,TResult> (DecodeWaysTests/PredictTheWinnerBenchmarks
// precedent) - each state's count of ways is cached so the same (index, runningSum) pair,
// reachable via many different +/- sign choices, is only ever solved once.
public sealed partial class TargetSumTests
{
    [Fact]
    public void FindTargetSumWays_ClassicExample_ReturnsFive()
    {
        int[] nums = [1, 1, 1, 1, 1];

        var ways = FindTargetSumWays(nums, target: 3);

        Assert.Equal(5, ways);
    }

    [Fact]
    public void FindTargetSumWays_SingleElementMatchingTarget_ReturnsOne()
    {
        int[] nums = [1];

        var ways = FindTargetSumWays(nums, target: 1);

        Assert.Equal(1, ways);
    }

    [Fact]
    public void FindTargetSumWays_UnreachableTarget_ReturnsZero()
    {
        int[] nums = [1, 2];

        var ways = FindTargetSumWays(nums, target: 100);

        Assert.Equal(0, ways);
    }

    private static int FindTargetSumWays(int[] nums, int target)
    {
        return Memoizer.Memoize<(int Index, int Sum), int>((0, 0), CountWays);

        int CountWays((int Index, int Sum) state, Func<(int Index, int Sum), int> ways)
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
