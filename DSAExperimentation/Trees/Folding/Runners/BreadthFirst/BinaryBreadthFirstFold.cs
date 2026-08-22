namespace DSAExperimentation.Trees;

public static class BinaryBreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TOrder, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
        => BreadthFirstWalk.Walk<
            TNode,
            BinaryChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TState>(root);
}
