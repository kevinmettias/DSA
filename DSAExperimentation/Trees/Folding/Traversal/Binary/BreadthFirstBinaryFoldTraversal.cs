namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstBinaryFoldTraversal<TNode>
    : IBinaryFoldTraversalStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
    {
        var discoveryOrder = new List<TNode>();
        var childrenByNode = new Dictionary<TNode, (TNode? First, TNode? Second)>();

        DiscoverNodes<TTopology, TOrder>(root, discoveryOrder, childrenByNode);

        return CombineBottomUp<TOrder, TAlgebra, TResult>(root, discoveryOrder, childrenByNode);
    }

    private static void DiscoverNodes<TTopology, TOrder>(
        TNode root,
        List<TNode> discoveryOrder,
        Dictionary<TNode, (TNode? First, TNode? Second)> childrenByNode)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
    {
        var queue = new Queue<TNode>();
        discoveryOrder.Add(root);
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var first = TOrder.GetFirst<TNode, TTopology>(node);
            var second = TOrder.GetSecond<TNode, TTopology>(node);

            if (first is not null)
            {
                discoveryOrder.Add(first);
                queue.Enqueue(first);
            }

            if (second is not null)
            {
                discoveryOrder.Add(second);
                queue.Enqueue(second);
            }

            childrenByNode[node] = (first, second);
        }
    }

    private static TResult CombineBottomUp<TOrder, TAlgebra, TResult>(
        TNode root,
        List<TNode> discoveryOrder,
        Dictionary<TNode, (TNode? First, TNode? Second)> childrenByNode)
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
    {
        var results = new Dictionary<TNode, TResult>();

        for (var i = discoveryOrder.Count - 1; i >= 0; i--)
        {
            var node = discoveryOrder[i];
            var (first, second) = childrenByNode[node];

            var firstResult = first is null ? TAlgebra.Empty : results[first];
            var secondResult = second is null ? TAlgebra.Empty : results[second];

            var children = TOrder.RestoreSemanticOrder(firstResult, secondResult);

            results[node] = TAlgebra.Combine(node, children);
        }

        return results[root];
    }
}
