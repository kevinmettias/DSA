using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PredictTheWinner;

// LeetCode 486. Predict the Winner: minimax interval DP over (left, right) bounds -
// the best score DIFFERENCE the player to move can force from nums[left..right],
// picking whichever end leaves the opponent the worse remaining sub-range. This
// repo's own Memoizer<TState,TResult> supplies the cache, keyed by that pair - the
// same (Left, Right)-state shape BurstBalloons/GuessNumberHigherOrLowerII already use
// for their own interval DP.
public sealed partial class PredictTheWinnerTests
{
    [Theory]
    [InlineData(new[] { 1, 5, 2 }, false)]
    [InlineData(new[] { 1, 5, 233, 7 }, true)]
    [InlineData(new[] { 1 }, true)]
    public void PredictTheWinner_LeetCodeExamples_ReturnsWhetherPlayerOneCanWinOrTie(int[] nums, bool expected)
        => Assert.Equal(expected, CanPlayerOneWin(nums));

    private static bool CanPlayerOneWin(int[] nums)
    {
        var scoreDiff = Memoizer.Memoize<(int Left, int Right), int>((0, nums.Length - 1), ScoreDiff);
        return scoreDiff >= 0;

        int ScoreDiff((int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
        {
            var (left, right) = range;
            if (left == right)
            {
                return nums[left];
            }

            var takeLeft = nums[left] - bestDiff((left + 1, right));
            var takeRight = nums[right] - bestDiff((left, right - 1));
            return Math.Max(takeLeft, takeRight);
        }
    }
}
