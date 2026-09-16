using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.UniqueBinarySearchTrees;

// LeetCode 96. Unique Binary Search Trees: count the structurally distinct
// BSTs that store values 1..n - the nth Catalan number, since a tree rooted
// at k pairs every one of the C(k-1) left shapes with every one of the
// C(n-k) right shapes, for each candidate root k.
//
// Both strategies compute the same recurrence; they differ only in
// direction. Tabulation builds it up bottom-up in a plain array. The
// memoized arm recurses top-down through this repo's own Memoizer instead,
// so each distinct node count is still solved only once - the same
// recurrence UniqueBinarySearchTreesIISolution drives to actually build the
// trees rather than just count them.
internal static class UniqueBinarySearchTreesSolution
{
    private const int FirstComposedNodeCount = 2;

    // Textbook baseline: dp[k] = # of distinct BSTs over k nodes, seeded with
    // the nodeCount = 0 (empty tree) and nodeCount = 1 (single node) base cases.
    // BCL array only.
    public static int CountTreesByTabulation(int nodeCount)
    {
        var dp = new int[nodeCount + 1];
        dp[0] = dp[1] = 1;

        for (var nodes = FirstComposedNodeCount; nodes <= nodeCount; nodes++)
        {
            for (var left = 0; left < nodes; left++)
            {
                dp[nodes] += dp[left] * dp[nodes - left - 1];
            }
        }

        return dp[nodeCount];
    }

    // Same split recurrence, driven top-down through Memoizer so every
    // distinct node count is computed once and shared across every caller
    // that needs it.
    public static int CountTreesByMemoizedCatalan(int nodeCount) =>
        Memoizer.Memoize<int, int>(nodeCount, new SplitAtEveryLeftCount());

    // The recurrence, named: a tree over `nodes` nodes is one candidate root per
    // left-subtree size, each pairing every left shape with every right shape, and
    // the two smallest counts are the base cases. Whether a repeated count is
    // computed once or many times is the memo run's business, not the rule's.
    private sealed class SplitAtEveryLeftCount : IRecurrence<int, int>
    {
        /// <inheritdoc/>
        public int Replay(int nodes, IRecurrence<int, int> rest)
        {
            if (nodes <= 1)
            {
                return 1;
            }

            var total = 0;

            for (var left = 0; left < nodes; left++)
            {
                total += rest.Replay(left, rest) * rest.Replay(nodes - left - 1, rest);
            }

            return total;
        }
    }
}
