namespace DSAExperimentation.Trees;

public readonly struct DepthFirstFoldTraversal<TNode>
    : IFoldTraversalStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => DepthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            TAlgebra,
            TResult>(root);
}
