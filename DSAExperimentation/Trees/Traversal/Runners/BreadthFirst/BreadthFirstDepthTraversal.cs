namespace DSAExperimentation.Trees;

public static class BreadthFirstDepthTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, TAction>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, IDepthAwareNodeAction<TNode>
        => BreadthFirstHookTraversal.Traverse<
            TNode,
            TTopology,
            TOrder,
            BreadthFirstDepthAwareNodeVisitHooks<TNode, TAction>>(root);
}