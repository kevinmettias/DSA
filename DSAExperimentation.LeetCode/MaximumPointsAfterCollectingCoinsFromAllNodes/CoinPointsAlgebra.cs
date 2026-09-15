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
// HalvingDepth.Max caps h: coins[i] <= 1e4 < 2^14, so coins[node] >> h is already 0
// for every h at or past HalvingDepth.Max, and folding h any deeper cannot change a
// further-halved contribution - the same "once it can't move the answer, stop
// tracking it more finely" reasoning RoomWaysPrecomputedFactorialAlgebra's own
// Prepare(n) table bound uses, just against a value magnitude instead of a tree size.
// Coins/k are external per-node data a static-abstract algebra cannot carry as
// instance state, so Prepare stashes them the same way
// RoomWaysPrecomputedFactorialAlgebra.Prepare stashes its factorial table before
// folding begins - in an AsyncLocal, so the stash belongs to the calling flow and a
// concurrent fold of the same problem cannot overwrite it part-way through the walk.
internal readonly struct CoinPointsAlgebra : IFoldAlgebra<RootedTreeNode, long[]>
{
    private static readonly AsyncLocal<CoinInputs> Inputs = new();

    private readonly record struct CoinInputs(long[] Coins, long K);

    public static long[] Empty => new long[HalvingDepth.Max + 1];

    public static void Prepare(int[] coins, int k)
    {
        var stored = new long[coins.Length];
        for (var i = 0; i < coins.Length; i++)
        {
            stored[i] = coins[i];
        }

        Inputs.Value = new CoinInputs(stored, k);
    }

    public static long[] Combine(RootedTreeNode node, IReadOnlyList<long[]> children)
    {
        var inputs = Inputs.Value;
        var coins = inputs.Coins[node.Id];
        var result = new long[HalvingDepth.Max + 1];

        for (var h = 0; h <= HalvingDepth.Max; h++)
        {
            result[h] = BestLevelValue(coins, inputs.K, h, children);
        }

        return result;
    }

    // This node's best outcome at one halving level: collect normally - coins shifted down
    // by that level, minus the flat cost, plus each child folded at the same level - or
    // halve here too - shifted down one level further with no cost, children folded one
    // level down as well. children[i][level] is dp(child i, level), already folded by the
    // time Combine is handed the child results.
    private static long BestLevelValue(
        long coins, long cost, int level, IReadOnlyList<long[]> children)
    {
        var nextLevel = Math.Min(level + 1, HalvingDepth.Max);
        var takeChildrenSum = 0L;
        var halveChildrenSum = 0L;

        for (var i = 0; i < children.Count; i++)
        {
            takeChildrenSum += children[i][level];
            halveChildrenSum += children[i][nextLevel];
        }

        var take = (coins >> level) - cost + takeChildrenSum;
        var halve = (coins >> nextLevel) + halveChildrenSum;

        return Math.Max(take, halve);
    }
}
