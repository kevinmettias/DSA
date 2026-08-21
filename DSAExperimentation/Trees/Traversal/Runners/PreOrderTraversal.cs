namespace DSAExperimentation.Trees;

public static class PreOrderTraversal
{
    public static void Traverse<TNode, TTopology, TOrder, TAction>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAction : struct, INodeAction<TNode>
    {
        DepthFirstTraversal.Traverse<
            TNode,
            TTopology,
            TOrder,
            PreOrderHooks<TNode, TAction>>(root);
    }
}
