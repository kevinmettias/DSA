namespace DSAExperimentation.Trees;

public readonly struct PreAndPostOrderDepthFirstHooks<TNode, TEnter, TExit>
    : IDepthFirstTraversalHooks<TNode, PreAndPostOrderDepthFirstHooks<TNode, TEnter, TExit>>
    where TEnter : struct, INodeAction<TNode>
    where TExit : struct, INodeAction<TNode>
{
    public static void OnEnter(TNode node)
        => NodeVisitHooks<TNode, TEnter>.OnEnter(node);

    public static void OnExit(TNode node)
        => PostOrderDepthFirstHooks<TNode, TExit>.OnExit(node);
}
