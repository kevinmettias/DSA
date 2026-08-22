namespace DSAExperimentation.Trees;

public static class BinaryDepthFirstFold
{
    public static TResult Fold<TNode, TTopology, TOrder, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TResult>
        => DepthFirstFold.Fold<
            TNode,
            BinaryChildOrderFoldStrategy<TNode, TTopology, TOrder, TAlgebra, TResult>,
            TResult>(root);
}
