using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVII;

// LeetCode 1690. Stone Game VII: the player to move from stones[Left..Right]
// removes either end stone and scores the sum of what remains - the same
// minimax interval-DP shape StoneGameTests/StoneGameVTests already use, keyed
// on the (Left, Right) pair and memoized by this repo's own
// Memoizer<TState,TResult>. Unlike LC 877's ScoreDiff (which scores the taken
// pile itself), here the score is the REMAINING sum after the removal, so both
// branches read from a prefix-sum array instead of a single stones[left]/
// stones[right] lookup - the recurrence's own doc comment below spells out why
// the returned value is still directly the winner-minus-loser difference.
public sealed class StoneGameVIITests
{
    [Fact]
    public void StoneValueDifference_LeetCodeExampleOne_ReturnsSix()
        => Assert.Equal(6, MaxScoreDifference([5, 3, 1, 4, 2]));

    [Fact]
    public void StoneValueDifference_LeetCodeExampleTwo_ReturnsOneHundredTwentyTwo()
        => Assert.Equal(122, MaxScoreDifference([7, 90, 5, 1, 100, 10, 10, 2]));

    [Fact]
    public void StoneValueDifference_TwoStones_RemovesSmallerToScoreTheLarger()
        => Assert.Equal(4, MaxScoreDifference([1, 4]));

    private static int MaxScoreDifference(int[] stones)
    {
        var n = stones.Length;
        var prefix = new int[n + 1];

        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + stones[i];
        }

        return Memoizer.Memoize<(int Left, int Right), int>((0, n - 1), Best);

        // Best(left,right) is the max score DIFFERENCE the player to move can force
        // from stones[left..right]. Removing the left stone scores the remaining
        // sum stones[left+1..right] now, minus whatever difference the opponent
        // forces from the resulting (left+1,right) sub-range - the standard
        // "your margin minus the opponent's best response" recurrence StoneGame/
        // StoneGameV already use, so the top-level call directly answers "the
        // difference between the winner's score and the loser's score."
        int Best((int Left, int Right) range, Func<(int, int), int> bestDiff)
        {
            var (left, right) = range;
            if (left == right)
            {
                return 0;
            }

            var removeLeftScore = prefix[right + 1] - prefix[left + 1];
            var removeRightScore = prefix[right] - prefix[left];

            var takeLeft = removeLeftScore - bestDiff((left + 1, right));
            var takeRight = removeRightScore - bestDiff((left, right - 1));
            return Math.Max(takeLeft, takeRight);
        }
    }
}
