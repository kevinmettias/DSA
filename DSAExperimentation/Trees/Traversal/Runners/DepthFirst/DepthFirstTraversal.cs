namespace DSAExperimentation.Trees;

public readonly struct DepthFirstTraversal<TNode, TTopology, TOrder, THooks>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where THooks : struct, IDepthFirstFoldAlgebra<TNode, Unit>
{
    public static void Traverse(TNode? root)
        => DepthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            THooks,
            Unit>(root);
}
