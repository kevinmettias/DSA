namespace DSAExperimentation.Trees;

public readonly struct PreAndPostOrderDepthFirstTraversal<TNode, TTopology, TOrder, TEnter, TExit>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where TEnter : struct, INodeAction<TNode>
    where TExit : struct, INodeAction<TNode>
{
    public static void Traverse(TNode? root)
        => DepthFirstTraversal<
            TNode,
            TTopology,
            TOrder,
            PreAndPostOrderDepthFirstHooks<TNode, TEnter, TExit>>.Traverse(root);
}
