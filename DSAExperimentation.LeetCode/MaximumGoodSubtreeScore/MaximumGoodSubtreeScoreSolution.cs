using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MaximumGoodSubtreeScore;

// LeetCode 3575. Maximum Good Subtree Score: par[] describes a tree rooted at node 0
// (LC's own parent-array shape - the same one CountWaysToBuildRoomsInAnAntColony
// consumes). For each node u, maxScore[u] is the largest sum of a subset of values
// drawn from u's subtree such that no decimal digit 0-9 is used twice across the
// chosen values; the answer is the sum of maxScore over every node, modulo 1e9+7.
internal static class MaximumGoodSubtreeScoreSolution
{
    // Per node, collect its subtree's values and brute-force every subset (2^size),
    // keeping the best digit-disjoint sum - the definition read literally, with none
    // of the composed strategy's cross-subtree reuse.
    public static int GoodSubtreeScoreSumByBruteForce(int[] vals, int[] par)
    {
        var children = BuildChildren(par);
        var total = 0L;

        for (var u = 0; u < vals.Length; u++)
        {
            var subtreeValues = new List<int>();
            CollectSubtree(u, vals, children, subtreeValues);
            total += BestGoodSubsetScore(subtreeValues);
        }

        return (int)(total % ModularArithmetic.Modulo);
    }

    private static List<int>[] BuildChildren(int[] par)
    {
        var children = new List<int>[par.Length];

        for (var i = 0; i < par.Length; i++)
        {
            children[i] = [];
        }

        for (var i = 0; i < par.Length; i++)
        {
            if (par[i] >= 0)
            {
                children[par[i]].Add(i);
            }
        }

        return children;
    }

    private static void CollectSubtree(int nodeId, int[] vals, List<int>[] children, List<int> values)
    {
        values.Add(vals[nodeId]);

        foreach (var child in children[nodeId])
        {
            CollectSubtree(child, vals, children, values);
        }
    }

    private static long BestGoodSubsetScore(List<int> values)
    {
        var best = 0L;

        for (var subset = 0; subset < (1 << values.Count); subset++)
        {
            var (isGood, sum) = EvaluateSubset(values, subset);

            if (isGood && sum > best)
            {
                best = sum;
            }
        }

        return best;
    }

    // What one candidate subset is worth: its values summed, and whether it is good at all -
    // no two of the values it picks may share a digit, so a repeated digit disqualifies the
    // whole subset and its sum never reaches the caller.
    private static (bool IsGood, long Sum) EvaluateSubset(List<int> values, int subset)
    {
        var digitsUsed = 0;
        var sum = 0L;

        for (var i = 0; i < values.Count; i++)
        {
            if ((subset & (1 << i)) == 0)
            {
                continue;
            }

            if (!TryDigitMask(values[i], out var valueMask) || (digitsUsed & valueMask) != 0)
            {
                return (false, sum);
            }

            digitsUsed |= valueMask;
            sum += values[i];
        }

        return (true, sum);
    }

    private static bool TryDigitMask(int value, out int mask)
    {
        mask = 0;

        while (value > 0)
        {
            var bit = 1 << (value % 10);

            if ((mask & bit) != 0)
            {
                mask = 0;
                return false;
            }

            mask |= bit;
            value /= 10;
        }

        return true;
    }

    // A TreeFold over DataStructures' own parent-array tree, closed over
    // GoodSubtreeScoreAlgebra: each node's Combine merges its children's already-
    // folded digit-mask knapsacks with its own value in O(1) merges of a 1024-entry
    // array, instead of re-enumerating every subtree's subsets from scratch.
    public static int GoodSubtreeScoreSumByBitmaskTreeFold(int[] vals, int[] par)
    {
        var root = ParentArrayTree.Build(par)[0];

        return GoodSubtreeScoreSumByBitmaskTreeFold(root, vals);
    }

    public static int GoodSubtreeScoreSumByBitmaskTreeFold(RootedTreeNode root, int[] vals)
    {
        GoodSubtreeScoreAlgebra.Prepare(vals);

        var (_, scoreSum) = TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            GoodSubtreeScoreAlgebra, (long[] Dp, long ScoreSum)>(root);

        return (int)(scoreSum % ModularArithmetic.Modulo);
    }
}
