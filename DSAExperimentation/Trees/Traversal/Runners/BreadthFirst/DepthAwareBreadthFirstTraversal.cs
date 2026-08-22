namespace DSAExperimentation.Trees;

public readonly struct DepthAwareBreadthFirstTraversal<TNode, TTopology, TOrder, TAction>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TAction : struct, IDepthAwareNodeAction<TNode>
{
    public static void Traverse(TNode? root)
        => BreadthFirstTraversal<
            TNode,
            TTopology,
            TOrder,
            DepthAwareBreadthFirstNodeVisitHooks<TNode, TAction>>.Traverse(root);
}
