namespace DSAExperimentation.Trees;

public readonly struct DepthAwareBreadthFirstNodeVisitHooks<TNode, TAction>
    : IBreadthFirstTraversalHooks<TNode, DepthAwareBreadthFirstNodeVisitHooks<TNode, TAction>>
    where TAction : struct, IDepthAwareNodeAction<TNode>
{
    public static void Visit(TNode node, int depth)
        => TAction.Invoke(node, depth);
}
