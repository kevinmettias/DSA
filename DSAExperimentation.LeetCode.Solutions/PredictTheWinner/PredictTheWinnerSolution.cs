using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.LeetCode.Solutions.PredictTheWinner;

// LeetCode 486. Predict the Winner: minimax interval DP over (left, right) bounds -
// the best score DIFFERENCE the player to move can force from nums[left..right],
// picking whichever end leaves the opponent the worse remaining sub-range. Player
// one can force a win or tie exactly when that difference is non-negative.
[LeetCodeQuestion("predict-the-winner", LeetCodeAnswerOrder.AsWritten)]
internal static class PredictTheWinnerSolution
{
    // The textbook baseline: plain recursion over (left, right) bounds with no
    // caching, so the same sub-range recurs through many different pick orders and
    // the cost is exponential. Deliberately written without this repo's primitives -
    // it is the arm the memoized strategy below has to justify itself against.
    public static bool CanWinByUnmemoizedRecursion(int[] nums) => ScoreDiff(nums, 0, nums.Length - 1) >= 0;

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the same
    // (Left, Right)-state shape BurstBalloons/GuessNumberHigherOrLowerII already use
    // for their own interval DP.
    public static bool CanWinByMemoizedRecursion(int[] nums)
    {
        var scoreDiff = Memoizer.Memoize<(int Left, int Right), int>(
            (0, nums.Length - 1),
            new ScoreDifferenceFrom(nums));

        return scoreDiff >= 0;
    }

    // The rule named as Domain's alternating game, valued by Algorithms/GameSearch's negamax:
    // the recursion above, with the minimax and its memo the library's. Player one wins or ties
    // when the difference they can force is not negative.
    public static bool CanWinByNegamax(int[] nums)
    {
        var difference = Negamax.Value<EndPickGame, (int Left, int Right), int>(
            new EndPickGame(nums), (0, nums.Length - 1));

        return difference >= 0;
    }

    // The recurrence itself, named: taking either end leaves the opponent the rest,
    // and what the player to move forces is that end's value minus whatever
    // difference the opponent then forces back.
    private sealed class ScoreDifferenceFrom(int[] nums) : IRecurrence<(int Left, int Right), int>
    {
        public int Replay((int Left, int Right) range, IRecurrence<(int Left, int Right), int> rest)
        {
            var (left, right) = range;

            if (left == right)
            {
                return nums[left];
            }

            var takeLeft = nums[left] - rest.Replay((left + 1, right), rest);
            var takeRight = nums[right] - rest.Replay((left, right - 1), rest);
            return Math.Max(takeLeft, takeRight);
        }
    }

    private static int ScoreDiff(int[] nums, int left, int right)
    {
        if (left == right)
        {
            return nums[left];
        }

        var takeLeft = nums[left] - ScoreDiff(nums, left + 1, right);
        var takeRight = nums[right] - ScoreDiff(nums, left, right - 1);
        return Math.Max(takeLeft, takeRight);
    }

    // The rule as an alternating game: from a range of scores a turn takes the score at either end,
    // move 0 the left one and move 1 the right; a single score is one move, and an empty range has
    // nothing left to take.
    private readonly struct EndPickGame(int[] nums) : IAlternatingGame<(int Left, int Right), int>
    {
        private const int TakeLeft = 0;
        private const int EndCount = 2;

        public int MoveCount((int Left, int Right) range) =>
            Math.Clamp(range.Right - range.Left + 1, 0, EndCount);

        public (int Left, int Right) Next((int Left, int Right) range, int move)
        {
            if (move == TakeLeft)
            {
                return (range.Left + 1, range.Right);
            }

            return (range.Left, range.Right - 1);
        }

        public int Gain((int Left, int Right) range, int move)
        {
            var taken = move == TakeLeft ? range.Left : range.Right;

            return nums[taken];
        }

        public int ValueWhenOver((int Left, int Right) range) => 0;
    }
}
