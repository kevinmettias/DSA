namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstNodeVisitHooks<TNode, TAction>
    : IBreadthFirstHooks<TNode>
    where TAction : struct, INodeAction<TNode>
{
    public static void Discover(TNode node, int depth)
    {
    }

    public static void Visit(TNode node, int depth)
        => TAction.Invoke(node);

    public static void Finish()
    {
    }
}




