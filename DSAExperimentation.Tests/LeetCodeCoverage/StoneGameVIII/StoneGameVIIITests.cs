using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVIII;

// LeetCode 1872. Stone Game VIII: each move removes the first x (x > 1) stones and
// replaces them with one stone valued at their sum, so - thanks to that replacement
// - the score of ANY move is simply the prefix sum up to whichever boundary index
// is chosen, and boundaries only need to strictly increase by at least one
// (the replacement stone itself always counts toward the next move's x >= 2). That
// collapses the game into a single backward chain over boundary indices, one state
// depending only on the next: best(i) = max(best(i+1), prefix[i] - best(i+1)) -
// "take this boundary now" vs. "let a later boundary's already-computed best carry
// forward unchanged" - memoized by this repo's own Memoizer<TState,TResult>, the
// same top-down interval-DP tool StoneGameVIITests already uses for a sibling
// problem, though here the state is a single index rather than an (left,right) pair.
public sealed class StoneGameVIIITests
{
    [Fact]
    public void StoneValueDifference_LeetCodeExampleOne_ReturnsFive()
        => Assert.Equal(5, MaxScoreDifference([-1, 2, -3, 4, -5]));

    [Fact]
    public void StoneValueDifference_LeetCodeExampleTwo_ReturnsThirteen()
        => Assert.Equal(13, MaxScoreDifference([7, -6, 5, 10, 5, -2, -6]));

    [Fact]
    public void StoneValueDifference_TwoStones_TakesBothInOneForcedMove()
        => Assert.Equal(-22, MaxScoreDifference([-10, -12]));

    private static long MaxScoreDifference(int[] stones)
    {
        var prefix = ComputePrefixSums(stones);

        // LeetCode guarantees stones.Length >= 2. The first move must remove x > 1
        // stones, i.e. its boundary index is at least 1 (taking stones[0] and
        // stones[1]) - so the answer is Best(1), not Best(0); Best's own recursion
        // handles every boundary from there onward.
        return Memoizer.Memoize<int, long>(1, (boundary, best) => Best(prefix, boundary, best));
    }

    private static long[] ComputePrefixSums(int[] stones)
    {
        var prefix = new long[stones.Length];
        prefix[0] = stones[0];

        for (var i = 1; i < stones.Length; i++)
        {
            prefix[i] = prefix[i - 1] + stones[i];
        }

        return prefix;
    }

    // Best(boundary) is the max score difference the player about to choose a
    // boundary >= `boundary` can force. At the last possible boundary (n-1, taking
    // every stone) there is no continuation left to compare against. Otherwise it's
    // "take this boundary now" (score prefix[boundary], minus whatever the opponent
    // forces from boundary+1 onward) vs. simply reusing the best choice available
    // from boundary+1 onward (deferring to a later boundary instead).
    private static long Best(long[] prefix, int boundary, Func<int, long> best)
    {
        if (boundary == prefix.Length - 1)
        {
            return prefix[boundary];
        }

        var takeHere = prefix[boundary] - best(boundary + 1);
        return Math.Max(best(boundary + 1), takeHere);
    }
}
