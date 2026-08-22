namespace DSAExperimentation.Trees;

public readonly struct NodeVisitBreadthFirstTraversal<TNode, TTopology, TOrder, TAction>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TAction : struct, INodeAction<TNode>
{
    public static void Traverse(TNode? root)
        => BreadthFirstTraversal<
            TNode,
            TTopology,
            TOrder,
            NodeVisitBreadthFirstHooks<TNode, TAction>>.Traverse(root);
}
