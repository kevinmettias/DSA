namespace DSAExperimentation.Trees;

public readonly struct PreOrderDepthFirstTraversal<TNode, TTopology, TOrder, TAction>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TAction : struct, INodeAction<TNode>
{
    public static void Traverse(TNode? root)
        => DepthFirstTraversal<
            TNode,
            TTopology,
            TOrder,
            NodeVisitHooks<TNode, TAction>>.Traverse(root);
}
