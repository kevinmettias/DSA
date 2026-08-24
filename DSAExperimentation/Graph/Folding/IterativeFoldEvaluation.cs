namespace DSAExperimentation.Graph;

// Discovers every node breadth-first (an explicit queue, not the call stack), then
// combines bottom-up in reverse discovery order. The breadth-first discovery is
// just the mechanism - what this buys you is that it won't stack-overflow on a
// deep, unbalanced tree the way DepthFirstFoldTraversal's recursion can. Per the
// purity caveat on IFoldAlgebra, it produces exactly the same result as
// RecursiveFoldEvaluation for any pure algebra; it's a different way to compute the
// same fold, not a different traversal order in any way a caller can observe.
public readonly struct IterativeFoldEvaluation<TNode> : IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var discoveryOrder = new List<TNode>();
        var childrenByNode = new Dictionary<TNode, List<TNode>>();

        DiscoverNodes<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
            root, discoveryOrder, childrenByNode);

        return CombineBottomUp<TAlgebra, TResult>(root, discoveryOrder, childrenByNode);
    }

    private static void DiscoverNodes<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode root,
        List<TNode> discoveryOrder,
        Dictionary<TNode, List<TNode>> childrenByNode)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var queue = new Queue<(TNode Node, int Depth)>();

        TAlgebra.Enter(root, 0);
        discoveryOrder.Add(root);
        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
            var children = new List<TNode>(orderedChildren.Count);

            for (var i = 0; i < orderedChildren.Count; i++)
            {
                var child = orderedChildren[i];
                TAlgebra.Enter(child, depth + 1);
                discoveryOrder.Add(child);
                children.Add(child);
                queue.Enqueue((child, depth + 1));
            }

            childrenByNode[node] = children;
        }
    }

    private static TResult CombineBottomUp<TAlgebra, TResult>(
        TNode root,
        List<TNode> discoveryOrder,
        Dictionary<TNode, List<TNode>> childrenByNode)
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var results = new Dictionary<TNode, TResult>();

        for (var i = discoveryOrder.Count - 1; i >= 0; i--)
        {
            var node = discoveryOrder[i];
            var children = childrenByNode[node];
            var childResults = new TResult[children.Count];

            for (var j = 0; j < children.Count; j++)
            {
                childResults[j] = results[children[j]];
            }

            results[node] = TAlgebra.Combine(node, childResults);
        }

        return results[root];
    }
}
