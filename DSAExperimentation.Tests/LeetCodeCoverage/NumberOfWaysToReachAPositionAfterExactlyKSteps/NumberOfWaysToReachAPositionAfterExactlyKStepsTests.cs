using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReachAPositionAfterExactlyKSteps;

// LeetCode 2400. Number of Ways to Reach a Position After Exactly k Steps: a
// top-down (remainingSteps, distanceToTarget) recursion memoized through this
// repo's own Memoizer<TState,TResult> - the exact same shape TargetSumTests'
// (index, runningSum) recursion already uses, just over a single collapsed
// distance instead of a running sum. distanceToTarget only ever needs its
// absolute value: from an offset of 0, both the left and right neighbor step
// land on the symmetric state |0-1|/|0+1| = 1, so the two recursive branches
// double-count that state's ways exactly the way the two physical step choices
// do. Pruning (diff > steps) is a plain early-exit, not a required correctness
// check - Ways(0, diff) already returns 0 for any diff != 0.
public sealed class NumberOfWaysToReachAPositionAfterExactlyKStepsTests
{
    private const int Mod = 1_000_000_007;

    [Fact]
    public void NumberOfWays_LeetCodeExampleOne_ReturnsThree()
    {
        var ways = NumberOfWays(startPos: 1, endPos: 2, k: 3);

        Assert.Equal(3, ways);
    }

    [Fact]
    public void NumberOfWays_LeetCodeExampleTwo_ReturnsZeroWhenUnreachable()
    {
        var ways = NumberOfWays(startPos: 2, endPos: 5, k: 10);

        Assert.Equal(0, ways);
    }

    [Fact]
    public void NumberOfWays_ZeroStepsAtTarget_ReturnsOne()
    {
        var ways = NumberOfWays(startPos: 4, endPos: 4, k: 0);

        Assert.Equal(1, ways);
    }

    private static int NumberOfWays(int startPos, int endPos, int k)
    {
        var diff = Math.Abs(endPos - startPos);
        return (int)Memoizer.Memoize<(int Steps, int Diff), long>((k, diff), Ways);
    }

    private static long Ways((int Steps, int Diff) state, Func<(int Steps, int Diff), long> ways)
    {
        var (steps, diff) = state;

        if (diff > steps)
        {
            return 0;
        }

        if (steps == 0)
        {
            return diff == 0 ? 1 : 0;
        }

        var towardTarget = ways((steps - 1, Math.Abs(diff - 1)));
        var awayFromTarget = ways((steps - 1, diff + 1));
        return (towardTarget + awayFromTarget) % Mod;
    }
}
