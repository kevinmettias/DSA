namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstFoldTraversal<TNode>
    : IFoldTraversalStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var discoveryOrder = new List<TNode>();
        var childrenByNode = new Dictionary<TNode, List<TNode>>();

        DiscoverNodes<TTopology, TOrder, TAlgebra, TResult>(root, discoveryOrder, childrenByNode);

        return CombineBottomUp<TAlgebra, TResult>(root, discoveryOrder, childrenByNode);
    }

    private static void DiscoverNodes<TTopology, TOrder, TAlgebra, TResult>(
        TNode root,
        List<TNode> discoveryOrder,
        Dictionary<TNode, List<TNode>> childrenByNode)
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var queue = new Queue<TraversalFrame<TNode>>();

        TAlgebra.Enter(root, 0);
        discoveryOrder.Add(root);
        queue.Enqueue(new(root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            var children = new List<TNode>();

            foreach (var child in ChildOrderStrategy<TNode, TTopology, TOrder>.GetChildren(node))
            {
                TAlgebra.Enter(child, depth + 1);
                discoveryOrder.Add(child);
                children.Add(child);
                queue.Enqueue(new(child, depth + 1));
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

            results[node] = TAlgebra.Combine(node, ChildResults<TAlgebra, TResult>(children, results));
        }

        return results[root];
    }

    private static IReadOnlyList<TResult> ChildResults<TAlgebra, TResult>(
        List<TNode> children,
        Dictionary<TNode, TResult> results)
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (!TAlgebra.CollectsChildResults || children.Count == 0)
        {
            return [];
        }

        var childResults = new TResult[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            childResults[i] = results[children[i]];
        }

        return childResults;
    }
}
