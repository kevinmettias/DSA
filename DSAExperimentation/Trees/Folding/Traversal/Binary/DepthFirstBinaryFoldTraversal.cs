namespace DSAExperimentation.Trees;

public readonly struct DepthFirstBinaryFoldTraversal<TNode>
    : IBinaryFoldTraversalStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
        => FoldNode<TTopology, TOrder, TAlgebra, TResult>(root);

    private static TResult FoldNode<TTopology, TOrder, TAlgebra, TResult>(TNode node)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
    {
        var first = FoldOrEmpty<TTopology, TOrder, TAlgebra, TResult>(TOrder.GetFirst<TNode, TTopology>(node));
        var second = FoldOrEmpty<TTopology, TOrder, TAlgebra, TResult>(TOrder.GetSecond<TNode, TTopology>(node));

        var children = TOrder.RestoreSemanticOrder(first, second);

        return TAlgebra.Combine(node, children);
    }

    private static TResult FoldOrEmpty<TTopology, TOrder, TAlgebra, TResult>(TNode? node)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
        => node is null
            ? TAlgebra.Empty
            : FoldNode<TTopology, TOrder, TAlgebra, TResult>(node);
}
