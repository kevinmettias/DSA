using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

// LeetCode 2920's per-node choice, folded bottom-up: at halving level h (how many
// ancestors already chose to halve on the way down to this node), either collect
// normally - coins[node] >> h, minus the flat cost k, plus every child folded at
// the SAME level h - or halve here too - coins[node] >> (h + 1), plus every child
// folded at level h + 1, with no cost. Combine returns the whole per-level table at
// once (index h is dp(node, h)) so a parent can read any level its own Combine
// needs without re-walking the subtree - exactly the DP this problem wants, shaped
// as one IFoldAlgebra.Combine per node instead of a hand-rolled recursion.
//
// MaxHalvings caps h: coins[i] <= 1e4 < 2^14, so coins[node] >> h is already 0 for
// every h >= MaxHalvings, and folding h any deeper cannot change a further-halved
// contribution - the same "once it can't move the answer, stop tracking it more
// finely" reasoning RoomWaysPrecomputedFactorialAlgebra's own Prepare(n) table
// bound uses, just against a value magnitude instead of a tree size. Coins/k are
// external per-node data a static-abstract algebra cannot carry as instance state,
// so Prepare stashes them the same way RoomWaysPrecomputedFactorialAlgebra.Prepare
// stashes its factorial table before folding begins.
internal readonly struct CoinPointsAlgebra : IFoldAlgebra<RootedTreeNode, long[]>
{
    internal const int MaxHalvings = 14;

    private static long[] _coins = [];
    private static long _k;

    public static long[] Empty => new long[MaxHalvings + 1];

    public static void Prepare(int[] coins, int k)
    {
        _coins = new long[coins.Length];
        for (var i = 0; i < coins.Length; i++)
        {
            _coins[i] = coins[i];
        }

        _k = k;
    }

    public static long[] Combine(RootedTreeNode node, IReadOnlyList<long[]> children)
    {
        var coins = _coins[node.Id];
        var result = new long[MaxHalvings + 1];

        for (var h = 0; h <= MaxHalvings; h++)
        {
            var nextH = Math.Min(h + 1, MaxHalvings);
            var takeChildrenSum = 0L;
            var halveChildrenSum = 0L;

            for (var i = 0; i < children.Count; i++)
            {
                takeChildrenSum += children[i][h];
                halveChildrenSum += children[i][nextH];
            }

            var take = (coins >> h) - _k + takeChildrenSum;
            var halve = (coins >> nextH) + halveChildrenSum;

            result[h] = Math.Max(take, halve);
        }

        return result;
    }
}
