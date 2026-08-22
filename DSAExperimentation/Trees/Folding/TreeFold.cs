namespace DSAExperimentation.Trees;

public static class TreeFold
{
    public static TResult Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => Fold<TNode, TTopology, TOrder, DepthFirstFoldTraversal<TNode>, TAlgebra, TResult>(root);

    public static TResult Fold<TNode, TTopology, TOrder, TTraversal, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TTraversal : struct, IFoldTraversalStrategy<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => root is null
            ? TAlgebra.Empty
            : TTraversal.Evaluate<TTopology, TOrder, TAlgebra, TResult>(root);
}
