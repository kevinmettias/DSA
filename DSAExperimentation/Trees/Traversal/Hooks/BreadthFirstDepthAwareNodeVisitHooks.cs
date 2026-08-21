namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstDepthAwareNodeVisitHooks<TNode, TAction>
    : IBreadthFirstHooks<TNode>
    where TAction : struct, IDepthAwareNodeAction<TNode>
{
    public static void Discover(TNode node, int depth)
    {
    }

    public static void Visit(TNode node, int depth)
        => TAction.Invoke(node, depth);
}




