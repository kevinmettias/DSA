using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClimbingStairs;

// LeetCode 70. Climbing Stairs: ways(n) = ways(n-1) + ways(n-2), the same shape as
// Fibonacci - natural-looking recursion via this repo's Memoizer, no hand-rolled
// cache.
public sealed partial class ClimbingStairsTests
{
    [Fact]
    public void CountWays_FiveSteps_ReturnsEightDistinctClimbSequences()
    {
        var ways = Memoizer.Memoize<int, int>(
            5, (stepCount, climb) => stepCount <= 1 ? 1 : WaysFromPreviousTwoSteps(stepCount, climb));

        Assert.Equal(8, ways);
    }

    private static int WaysFromPreviousTwoSteps(int stepCount, Func<int, int> climb)
        => climb(stepCount - 1) + climb(stepCount - 2);
}
