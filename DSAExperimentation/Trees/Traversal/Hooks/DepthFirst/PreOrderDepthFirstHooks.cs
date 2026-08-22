namespace DSAExperimentation.Trees;

public readonly struct PreOrderDepthFirstHooks<TNode, TAction>
    : IDepthFirstTraversalHooks<TNode, PreOrderDepthFirstHooks<TNode, TAction>>
    where TAction : struct, INodeAction<TNode>
{
    public static void OnEnter(TNode node)
        => TAction.Invoke(node);
}
