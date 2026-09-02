using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumGoodSubtreeScore;

// LC 3575's per-subtree knapsack: a subset of a subtree is "good" only if no decimal
// digit 0-9 is used by more than one of its chosen values, so Dp[mask] is the best
// sum reachable using exactly `mask` as the union of chosen values' digits. Merging
// two subtrees' Dp arrays is a disjoint-mask knapsack merge, one array per child plus
// one for this node's own value, exactly the way a 0/1 knapsack merges items - mask
// has only 2^10 = 1024 states because there are only 10 decimal digits.
//
// TResult also carries ScoreSum, a running total of maxScore (= max(Dp)) over every
// node visited so far in this subtree - the fold's own accumulator, since
// IFoldAlgebra.Combine only ever returns the root's own value and the problem asks
// for the sum over every node, not just the root.
//
// This algebra answers one LeetCode problem and nothing else, which is why it lives
// beside the solution rather than in Domain (ARCHITECTURE.md 17.3), the same shape
// CountWaysToBuildRoomsInAnAntColony's RoomWaysAlgebra already takes.
internal readonly struct GoodSubtreeScoreAlgebra : IFoldAlgebra<RootedTreeNode, (long[] Dp, long ScoreSum)>
{
    private const int DigitCount = 10;
    private const int MaskCount = 1 << DigitCount;
    private const long Unreachable = long.MinValue / 2;

    private static int[] _vals = [];

    public static (long[] Dp, long ScoreSum) Empty => (EmptyDp(), 0);

    // The caller states every node's value up front, indexed by RootedTreeNode.Id -
    // the same "external state stashed before the fold starts" shape
    // RoomWaysPrecomputedFactorialAlgebra.Prepare uses, since a static-abstract
    // algebra carries no instance state of its own.
    public static void Prepare(int[] vals) => _vals = vals;

    public static (long[] Dp, long ScoreSum) Combine(
        RootedTreeNode node, IReadOnlyList<(long[] Dp, long ScoreSum)> children)
    {
        var merged = EmptyDp();
        var childScoreSum = 0L;

        foreach (var (childDp, scoreSum) in children)
        {
            merged = MergeDisjoint(merged, childDp);
            childScoreSum += scoreSum;
        }

        merged = MergeDisjoint(merged, OwnValueDp(node));

        return (merged, childScoreSum + MaxOf(merged));
    }

    // A one-item "child" that either contributes this node's own value or nothing -
    // merged through the same disjoint-mask knapsack as a real child, so a value
    // whose own digits repeat (its mask computation fails) simply leaves nothing
    // beyond the always-present empty choice.
    private static long[] OwnValueDp(RootedTreeNode node)
    {
        var dp = EmptyDp();
        var value = _vals[node.Id];

        if (TryDigitMask(value, out var mask))
        {
            dp[mask] = value;
        }

        return dp;
    }

    private static long[] MergeDisjoint(long[] left, long[] right)
    {
        var next = EmptyDp();

        for (var leftMask = 0; leftMask < MaskCount; leftMask++)
        {
            if (left[leftMask] == Unreachable)
            {
                continue;
            }

            for (var rightMask = 0; rightMask < MaskCount; rightMask++)
            {
                if (right[rightMask] == Unreachable || (leftMask & rightMask) != 0)
                {
                    continue;
                }

                var combinedMask = leftMask | rightMask;
                var combinedScore = left[leftMask] + right[rightMask];

                if (combinedScore > next[combinedMask])
                {
                    next[combinedMask] = combinedScore;
                }
            }
        }

        return next;
    }

    private static long MaxOf(long[] dp)
    {
        var best = 0L;

        foreach (var score in dp)
        {
            if (score > best)
            {
                best = score;
            }
        }

        return best;
    }

    private static long[] EmptyDp()
    {
        var dp = new long[MaskCount];
        Array.Fill(dp, Unreachable);
        dp[0] = 0;
        return dp;
    }

    private static bool TryDigitMask(int value, out int mask)
    {
        mask = 0;

        while (value > 0)
        {
            var bit = 1 << (value % DigitCount);

            if ((mask & bit) != 0)
            {
                mask = 0;
                return false;
            }

            mask |= bit;
            value /= DigitCount;
        }

        return true;
    }
}
