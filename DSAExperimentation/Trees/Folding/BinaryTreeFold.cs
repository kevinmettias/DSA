namespace DSAExperimentation.Trees;

public static class BinaryTreeFold
{
    public static TResult Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TOrder, DepthFirstBinaryFoldTraversal<TNode>, TAlgebra, TResult>(root);

    public static TResult Fold<TNode, TTopology, TOrder, TTraversal, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TTraversal : struct, IBinaryFoldTraversalStrategy<TNode>
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>
        => root is null
            ? TAlgebra.Empty
            : TTraversal.Evaluate<TTopology, TOrder, TAlgebra, TResult>(root);
}
