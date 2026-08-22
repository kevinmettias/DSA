namespace DSAExperimentation.Trees;

public static class BreadthFirstFold
{
    public static TState Fold<TNode, TTopology, TOrder, TAlgebra, TState>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IBreadthFirstFoldAlgebra<TNode, TState>
        => BreadthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TState>(root);
}
