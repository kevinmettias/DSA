using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGameV;

// LeetCode 1563. Stone Game V: repeatedly split the remaining row of stones into two
// non-empty rows, score (and keep splitting) the row with the smaller sum - either
// row on a tie - and discard the other, until one stone remains.
//
// The recurrence is an interval DP over (Left, Right): for every split point the
// smaller-summed side is scored and recursed into, and the answer is the best split.
// A prefix-sum table makes each side's sum O(1), so the two strategies differ only in
// whether the (Left, Right) states are cached - the same memoize-or-not pairing
// StoneGameIIISolution (LC 1406) runs, keyed on a range instead of a single index.
internal static class StoneGameVSolution
{
    // The textbook baseline: plain interval recursion with no caching. The same
    // (left, right) range is re-entered from many different parent splits - Best(0, 3)
    // is reachable directly and as the kept half of several larger ranges - so the
    // call count grows far past the O(n^2) distinct-range count. Deliberately written
    // without this repo's primitives: it is the arm the memoized strategy has to
    // justify itself against.
    public static int MaxScoreByUnmemoizedRecursion(int[] stoneValue)
    {
        var prefix = BuildPrefixSums(stoneValue);

        return Best(prefix, 0, stoneValue.Length - 1);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the exact
    // (Left, Right) range the recurrence branches on, so each of the O(n^2) distinct
    // ranges is evaluated once instead of once per parent split that reaches it.
    public static int MaxScoreByMemoizedRecursion(int[] stoneValue)
    {
        var prefix = BuildPrefixSums(stoneValue);

        return Memoizer.Memoize<(int Left, int Right), int>(
            (0, stoneValue.Length - 1),
            new BestSplitsOverPrefix(prefix));
    }

    // The recurrence, as a named type: the best score reachable from one (Left, Right)
    // range. The prefix-sum table it reads arrives through the primary constructor and
    // the memoized continuation through `rest`, so neither travels as a delegate.
    private sealed class BestSplitsOverPrefix(int[] prefix)
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

            var result = 0;

            for (var mid = left; mid < right; mid++)
            {
                result = BestForSplit((left, mid, right), result, rest);
            }

            return result;
        }

        // A split is the range together with the point inside it, so all three travel
        // as one argument rather than as a range plus a stray index.
        private int BestForSplit(
            (int Left, int Mid, int Right) split,
            int result,
            IRecurrence<(int Left, int Right), int> rest)
        {
            var (left, mid, right) = split;
            var leftSum = prefix[mid + 1] - prefix[left];
            var rightSum = prefix[right + 1] - prefix[mid + 1];

            if (leftSum <= rightSum)
            {
                result = Math.Max(result, leftSum + rest.Replay((left, mid), rest));
            }

            if (rightSum <= leftSum)
            {
                result = Math.Max(result, rightSum + rest.Replay((mid + 1, right), rest));
            }

            return result;
        }
    }

    private static int Best(int[] prefix, int left, int right)
    {
        if (left >= right)
        {
            return 0;
        }

        var result = 0;

        for (var mid = left; mid < right; mid++)
        {
            result = BestForSplit(prefix, (left, mid, right), result);
        }

        return result;
    }

    // Whichever side of the split at `mid` sums to no more than the other is the one
    // scored (and recursed into) this round, per Stone Game V's rule that a tie lets
    // either side be kept.
    private static int BestForSplit(int[] prefix, (int Left, int Mid, int Right) split, int result)
    {
        var (left, mid, right) = split;
        var leftSum = prefix[mid + 1] - prefix[left];
        var rightSum = prefix[right + 1] - prefix[mid + 1];

        if (leftSum <= rightSum)
        {
            result = Math.Max(result, leftSum + Best(prefix, left, mid));
        }

        if (rightSum <= leftSum)
        {
            result = Math.Max(result, rightSum + Best(prefix, mid + 1, right));
        }

        return result;
    }

    private static int[] BuildPrefixSums(int[] stoneValue)
    {
        var prefix = new int[stoneValue.Length + 1];

        for (var i = 0; i < stoneValue.Length; i++)
        {
            prefix[i + 1] = prefix[i] + stoneValue[i];
        }

        return prefix;
    }
}
