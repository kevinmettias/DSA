namespace DSAExperimentation.Trees;

public static class BreadthFirstReduce
{
    public static TState Reduce<TNode, TTopology, TOrder, TAlgebra, TState>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IBreadthFirstReduceAlgebra<TNode, TState>
        => BreadthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TState>(root);
}
