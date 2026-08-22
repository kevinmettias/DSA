namespace DSAExperimentation.Trees;

public readonly struct NodeVisitHooks<TNode, TAction>
    : IDepthFirstTraversalHooks<TNode, NodeVisitHooks<TNode, TAction>>,
      IBreadthFirstNodeVisitHooks<TNode, NodeVisitHooks<TNode, TAction>>
    where TAction : struct, INodeAction<TNode>
{
    public static void OnEnter(TNode node)
        => TAction.Invoke(node);

    public static void OnVisit(TNode node)
        => TAction.Invoke(node);
}
