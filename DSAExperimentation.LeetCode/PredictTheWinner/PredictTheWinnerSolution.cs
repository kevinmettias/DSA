using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PredictTheWinner;

// LeetCode 486. Predict the Winner: minimax interval DP over (left, right) bounds -
// the best score DIFFERENCE the player to move can force from nums[left..right],
// picking whichever end leaves the opponent the worse remaining sub-range. Player
// one can force a win or tie exactly when that difference is non-negative.
internal static class PredictTheWinnerSolution
{
    // The textbook baseline: plain recursion over (left, right) bounds with no
    // caching, so the same sub-range recurs through many different pick orders and
    // the cost is exponential. Deliberately written without this repo's primitives -
    // it is the arm the memoized strategy below has to justify itself against.
    public static bool CanWinByUnmemoizedRecursion(int[] nums) => ScoreDiff(nums, 0, nums.Length - 1) >= 0;

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

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the same
    // (Left, Right)-state shape BurstBalloons/GuessNumberHigherOrLowerII already use
    // for their own interval DP.
    public static bool CanWinByMemoizedRecursion(int[] nums)
    {
        var scoreDiff = Memoizer.Memoize<(int Left, int Right), int>(
            (0, nums.Length - 1),
            (range, bestDiff) => ScoreDiffMemoized(nums, range, bestDiff));

        return scoreDiff >= 0;
    }

    private static int ScoreDiffMemoized(int[] nums, (int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
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
