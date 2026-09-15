using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGameVII;

// LeetCode 1690. Stone Game VII: the player to move from stones[left..right] removes
// either end stone and scores the sum of what REMAINS, until one stone is left. Both
// play optimally and the answer is the winner's score minus the loser's.
//
// The recurrence is an interval DP over (Left, Right): Best(left, right) is the score
// difference the player to move can force, so removing an end scores the remaining
// sum now and then subtracts whatever difference the opponent forces from the smaller
// range - the "your margin minus the opponent's best response" shape StoneGameSolution
// (LC 877) and StoneGameVSolution (LC 1563) already use, which is why the top-level
// call is directly the winner-minus-loser answer rather than needing a second pass.
//
// Unlike LC 877's ScoreDiff (which scores the pile that was taken), the score here is
// the sum of what is left behind, so both branches read a prefix-sum table instead of
// a single stones[left]/stones[right] lookup. The two strategies differ only in
// whether the (Left, Right) states are cached.
internal static class StoneGameVIISolution
{
    // The textbook baseline: plain interval recursion with no caching. Every removal
    // order that reaches the same (left, right) range re-evaluates it from scratch, so
    // the call count is exponential in the pile count rather than the O(n^2) distinct
    // ranges. Deliberately written with BCL arrays only - it is the arm the memoized
    // strategy has to justify itself against.
    public static int MaxScoreDifferenceByUnmemoizedRecursion(int[] stones)
    {
        var prefix = BuildPrefixSums(stones);

        return Best(prefix, 0, stones.Length - 1);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the exact
    // (Left, Right) range the recurrence branches on, so each of the O(n^2) distinct
    // ranges is evaluated once instead of once per removal order that reaches it.
    public static int MaxScoreDifferenceByMemoizedRecursion(int[] stones)
    {
        var prefix = BuildPrefixSums(stones);

        return Memoizer.Memoize<(int Left, int Right), int>(
            (0, stones.Length - 1),
            new ScoreDifferencesOverPrefix(prefix));
    }

    // The recurrence, as a named type: the score difference the mover can force from one
    // (Left, Right) range. The prefix-sum table it reads arrives through the primary
    // constructor and the memoized continuation through `rest`, so neither is a delegate.
    private sealed class ScoreDifferencesOverPrefix(int[] prefix)
        : IRecurrence<(int Left, int Right), int>
    {
        public int Replay(
            (int Left, int Right) range, IRecurrence<(int Left, int Right), int> rest)
        {
            var (left, right) = range;

            if (left >= right)
            {
                return 0;
            }

            var takeLeft = RemoveLeftScore(prefix, left, right) - rest.Replay((left + 1, right), rest);
            var takeRight = RemoveRightScore(prefix, left, right) - rest.Replay((left, right - 1), rest);

            return Math.Max(takeLeft, takeRight);
        }
    }

    private static int Best(int[] prefix, int left, int right)
    {
        if (left >= right)
        {
            return 0;
        }

        var takeLeft = RemoveLeftScore(prefix, left, right) - Best(prefix, left + 1, right);
        var takeRight = RemoveRightScore(prefix, left, right) - Best(prefix, left, right - 1);

        return Math.Max(takeLeft, takeRight);
    }

    // Removing stones[left] leaves stones[left+1..right], which is what the mover scores.
    private static int RemoveLeftScore(int[] prefix, int left, int right)
        => prefix[right + 1] - prefix[left + 1];

    // Removing stones[right] leaves stones[left..right-1].
    private static int RemoveRightScore(int[] prefix, int left, int right)
        => prefix[right] - prefix[left];

    private static int[] BuildPrefixSums(int[] stones)
    {
        var prefix = new int[stones.Length + 1];

        for (var i = 0; i < stones.Length; i++)
        {
            prefix[i + 1] = prefix[i] + stones[i];
        }

        return prefix;
    }
}
