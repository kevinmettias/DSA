namespace DSAExperimentation.Trees;

public readonly struct NodeVisitBreadthFirstHooks<TNode, TAction>
    : IBreadthFirstTraversalHooks<TNode, NodeVisitBreadthFirstHooks<TNode, TAction>>
    where TAction : struct, INodeAction<TNode>
{
    public static void Visit(TNode node, int depth)
        => TAction.Invoke(node);
}
