namespace DSAExperimentation.Trees;

internal readonly struct BinaryChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>
    : IFoldStrategy<TNode, TResult>
    where TNode : class
    where TTopology : struct, IBinaryTreeTopology<TNode>
    where TOrder : struct, IBinaryChildOrder
    where TAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TResult>
{
    public static TResult Empty => TAlgebra.Empty;

    public static TResult FoldNode(TNode node)
    {
        var first = DepthFirstFold.Fold<
            TNode,
            BinaryChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>,
            TResult>(TOrder.GetFirst<TNode, TTopology>(node));

        var second = DepthFirstFold.Fold<
            TNode,
            BinaryChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>,
            TResult>(TOrder.GetSecond<TNode, TTopology>(node));

        var children = TOrder.RestoreSemanticOrder(first, second);

        return TAlgebra.Combine(
            node,
            children);
    }
}
