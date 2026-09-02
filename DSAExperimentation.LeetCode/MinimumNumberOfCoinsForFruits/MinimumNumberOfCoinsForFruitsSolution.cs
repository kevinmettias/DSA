using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumNumberOfCoinsForFruits;

// LeetCode 2944. Minimum Number of Coins for Fruits: prices is 1-indexed - buying
// the i-th fruit for prices[i-1] coins also makes the next i fruits free, but a
// free fruit can still be repurchased to unlock a fresh reward. dp[i] is the
// minimum cost to acquire fruits i..n given fruits before i are already covered;
// buying fruit i is forced (that is what "not yet covered" means), and its reward
// range folds every later purchase decision into one range-minimum lookup over
// dp[i+1 .. min(n, 2i)+1] (dp[n+1] = 0 is the "nothing left to buy" base case).
//
// Both strategies compute the exact same recurrence; they differ only in how that
// range minimum over dp is answered.
internal static class MinimumNumberOfCoinsForFruitsSolution
{
    // The textbook O(n^2) form: no structure at all, just rescan the window on
    // every dp[i]. The arm the segment-tree strategy below has to beat.
    public static int MinCoinsByBruteForceDp(int[] prices)
    {
        var n = prices.Length;
        var dp = new int[n + 2];

        for (var i = n; i >= 1; i--)
        {
            var right = Math.Min(n + 1, (2 * i) + 1);
            var best = dp[right];

            for (var j = i + 1; j < right; j++)
            {
                best = Math.Min(best, dp[j]);
            }

            dp[i] = prices[i - 1] + best;
        }

        return dp[1];
    }

    // Same recurrence, but dp[i+1 .. min(n, 2i)+1]'s minimum is one O(log n) query
    // against this repo's own SegmentTree<int, MinOperation<int>> instead of an
    // O(window) rescan. dp is filled right-to-left, so every index a query touches
    // has already been written by the time it is read.
    public static int MinCoinsBySegmentTreeDp(int[] prices)
    {
        var n = prices.Length;
        var initial = new int[n + 2];
        Array.Fill(initial, MinOperation<int>.Identity);
        initial[n + 1] = 0;

        var tree = new SegmentTree<int, MinOperation<int>>(initial);

        for (var i = n; i >= 1; i--)
        {
            var right = Math.Min(n + 1, (2 * i) + 1);
            var best = tree.Query(i + 1, right);

            tree.Update(i, prices[i - 1] + best);
        }

        return tree.Query(1, 1);
    }
}
