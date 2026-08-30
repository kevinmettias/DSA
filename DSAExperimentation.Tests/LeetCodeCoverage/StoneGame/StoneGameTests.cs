using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGame;

// LeetCode 877. Stone Game: the same minimax interval-DP shape as
// PredictTheWinnerTests (LC 486) - the best score DIFFERENCE the player to move
// can force from piles[left..right], memoized by this repo's own
// Memoizer<TState,TResult> keyed on the (Left, Right) pair. Stone Game's own
// constraints (an even pile count, all-positive pile sizes, so the total is
// never split evenly) guarantee Alice can always force a strictly positive
// difference - this test still runs the real recurrence rather than hardcoding
// true, so a regression in ScoreDiff itself would still be caught.
public sealed partial class StoneGameTests
{
    [Theory]
    [InlineData(new[] { 5, 3, 4, 5 }, true)]
    [InlineData(new[] { 3, 7, 2, 3 }, true)]
    [InlineData(new[] { 3, 2 }, true)]
    public void StoneGame_LeetCodeExamples_ReturnsWhetherAliceWins(int[] piles, bool expected)
        => Assert.Equal(expected, AliceWins(piles));

    private static bool AliceWins(int[] piles)
    {
        var scoreDiff = Memoizer.Memoize<(int Left, int Right), int>((0, piles.Length - 1), ScoreDiff);
        return scoreDiff > 0;

        int ScoreDiff((int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
        {
            var (left, right) = range;
            if (left == right)
            {
                return piles[left];
            }

            var takeLeft = piles[left] - bestDiff((left + 1, right));
            var takeRight = piles[right] - bestDiff((left, right - 1));
            return Math.Max(takeLeft, takeRight);
        }
    }
}
