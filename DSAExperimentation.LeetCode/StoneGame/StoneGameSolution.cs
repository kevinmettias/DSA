using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGame;

// LeetCode 877. Stone Game: the same minimax interval DP as LC 486 (see
// PredictTheWinnerSolution) - the best score DIFFERENCE the player to move can
// force from piles[left..right], picking whichever end leaves the opponent the
// worse remaining sub-range - with a different win condition on the result. Stone
// Game's own constraints (an even pile count, all-positive pile sizes, so the
// total can never split evenly) guarantee Alice can force a strictly positive
// difference; both strategies still run the real recurrence rather than returning
// a constant, so a regression in the difference itself is still caught.
internal static class StoneGameSolution
{
    // The textbook baseline: plain recursion over (left, right) bounds with no
    // caching, so the same sub-range recurs through many different pick orders and
    // the cost is exponential. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static bool AliceWinsByUnmemoizedRecursion(int[] piles) => ScoreDiff(piles, 0, piles.Length - 1) > 0;

    private static int ScoreDiff(int[] piles, int left, int right)
    {
        if (left == right)
        {
            return piles[left];
        }

        var takeLeft = piles[left] - ScoreDiff(piles, left + 1, right);
        var takeRight = piles[right] - ScoreDiff(piles, left, right - 1);
        return Math.Max(takeLeft, takeRight);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the
    // same (Left, Right)-state shape PredictTheWinner/BurstBalloons already use for
    // their own interval DP, collapsing the exponential recursion to O(n^2)
    // distinct sub-ranges.
    public static bool AliceWinsByMemoizedRecursion(int[] piles)
    {
        var scoreDiff = Memoizer.Memoize<(int Left, int Right), int>(
            (0, piles.Length - 1),
            (range, bestDiff) => ScoreDiffMemoized(piles, range, bestDiff));

        return scoreDiff > 0;
    }

    private static int ScoreDiffMemoized(int[] piles, (int Left, int Right) range, Func<(int Left, int Right), int> bestDiff)
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
