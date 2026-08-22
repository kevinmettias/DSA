namespace DSAExperimentation.Trees;

public static class DepthFirstFold
{
    public static TResult Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IDepthFirstFoldAlgebra<TNode, TResult>
        => DepthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TResult>(root);

    internal static TResult Fold<TNode, TFold, TResult>(TNode? root)
        where TNode : class
        where TFold : struct, IFoldStrategy<TNode, TResult>
        => root is null
            ? TFold.Empty
            : TFold.FoldNode(root);
}
