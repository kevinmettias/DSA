namespace DSAExperimentation.Trees;

public static class BinaryBreadthFirstReduce
{
    public static TState Reduce<TNode, TTopology, TOrder, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBreadthFirstReduceAlgebra<TNode, TState>
        => BreadthFirstWalk.Walk<
            TNode,
            BinaryChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TState>(root);
}
