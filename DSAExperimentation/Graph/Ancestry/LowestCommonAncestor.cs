namespace DSAExperimentation.Graph;

// Bespoke, like Dijkstra: reuses the topology/children/order contracts for
// adjacency-shape genericity, but is NOT expressed as an IFoldAlgebra. Every member
// of IFoldAlgebra/IReduceAlgebra is static-abstract - fixed by TYPE, with no channel
// to close over a RUNTIME value - so there's no way for Combine to know "which two
// nodes am I looking for." SizeAlgebra/HeightAlgebra/DiameterAlgebra never needed
// this because they're purely structural; LCA is the first genuinely
// runtime-parameterized query in this library, and it needs to branch on that
// parameter mid-recursion (stop descending once p or q is found), so deferring the
// check to after a structural pass (see GridShortestPath.Distance) isn't an option
// here either. The fix is the same one Dijkstra already uses: write the recursion
// directly and close over p/q as ordinary parameters, the way any hand-written
// function would.
public static class LowestCommonAncestor
{
    public static TNode? Find<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode root, TNode p, TNode q)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        => Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(root, p, q);

    private static TNode? Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode node, TNode p, TNode q)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        if (node.Equals(p) || node.Equals(q))
        {
            return node;
        }

        var children = TOrder.Apply(TTopology.GetChildren(node));
        TNode? foundInOneChild = null;
        var subtreesWithAMatch = 0;

        for (var i = 0; i < children.Count; i++)
        {
            var result = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(children[i], p, q);

            if (result is not null)
            {
                foundInOneChild = result;
                subtreesWithAMatch++;
            }
        }

        // p and q turning up in two different children means this node is where
        // their paths from the root diverge - the LCA. Generalizes past binary
        // trees for free: it's "two different subtrees", not "left and right".
        return subtreesWithAMatch >= 2 ? node : foundInOneChild;
    }
}
