namespace DSAExperimentation.Trees;

public readonly struct PostOrderDepthFirstHooks<TNode, TAction>
    : IDepthFirstTraversalHooks<TNode, PostOrderDepthFirstHooks<TNode, TAction>>
    where TAction : struct, INodeAction<TNode>
{
    public static void OnExit(TNode node)
        => TAction.Invoke(node);
}
