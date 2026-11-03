using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Ancestry;

// Bespoke, like Dijkstra: reuses the topology/children/order contracts for
// adjacency-shape genericity, but is NOT expressed as an IFoldAlgebra. The runtime
// query is no obstacle - an algebra can carry first and second as fields (see
// IFoldAlgebra) - but a fold cannot stop early: every child is folded before Combine
// sees its parent, so a fold would walk the whole subtree beneath first or second,
// where this recursion returns at the match without descending. The answer would be
// the same; the walk below each match would be wasted. Nor is it an IReduceAlgebra.
// A reduce's state can carry first and second, and IsDone can end the walk -
// BreadthFirstDistances.To does both - but IsDone ends the whole walk: it cannot skip
// the subtree below one match and keep walking the rest, and that cut is the point.
// So it does what Dijkstra does: write the recursion directly and close over
// first/second as ordinary parameters, the way any hand-written function would.
/// <summary>The lowest common ancestor of two tree nodes: the deepest node whose subtree holds both.</summary>
public static class LowestCommonAncestor
{
    // first and second turning up in two different children means this node is where
    // their paths from the root diverge - the LCA. Generalizes past binary trees for
    // free: it's "two different subtrees", not "left and right".
    private const int DivergingSubtreeThreshold = 2;

    /// <summary>Finds two nodes' lowest common ancestor in <c>TOrder</c>'s child order.</summary>
    /// <param name="root">The root of the tree to search.</param>
    /// <param name="first">One node to look for, matched with <c>Equals</c>.</param>
    /// <param name="second">The other node to look for, matched with <c>Equals</c>.</param>
    /// <returns>
    /// Their lowest common ancestor; the one found when only one is in the tree; null when neither is.
    /// </returns>
    public static TNode? Find<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode root, TNode first, TNode second)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(root, first, second);

    // In the topology's own child order.
    /// <summary>Finds two nodes' lowest common ancestor in the topology's own child order.</summary>
    /// <param name="root">The root of the tree to search.</param>
    /// <param name="first">One node to look for, matched with <c>Equals</c>.</param>
    /// <param name="second">The other node to look for, matched with <c>Equals</c>.</param>
    /// <returns>
    /// Their lowest common ancestor; the one found when only one is in the tree; null when neither is.
    /// </returns>
    public static TNode? Find<TNode, TTopology, TChildren>(TNode root, TNode first, TNode second)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        => Find<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren>(root, first, second);

    private static TNode? Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode node, TNode first, TNode second)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => node.Equals(first) || node.Equals(second)
            ? node
            : FindDivergencePoint<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, first, second);

    private static TNode? FindDivergencePoint<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode node, TNode first, TNode second)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var children = TOrder.Apply(TTopology.GetChildren(node));
        TNode? foundInOneChild = null;
        var subtreesWithAMatch = 0;

        for (var i = 0; i < children.Count; i++)
        {
            var result = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(children.Get(i), first, second);

            if (result is not null)
            {
                foundInOneChild = result;
                subtreesWithAMatch++;
            }
        }

        return subtreesWithAMatch >= DivergingSubtreeThreshold ? node : foundInOneChild;
    }
}
