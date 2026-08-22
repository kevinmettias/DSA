namespace DSAExperimentation.Trees;

public readonly struct BinaryBreadthFirstTraversal<TNode, TTopology, TOrder, THooks>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, IBinaryTreeTopology<TNode>
    where TOrder : struct, IBinaryChildOrder
    where THooks : struct, IBreadthFirstFoldAlgebra<TNode, Unit>
{
    public static void Traverse(TNode? root)
        => BreadthFirstWalk.Walk<
            TNode,
            BinaryChildOrderStrategy<TNode, TTopology, TOrder>,
            THooks,
            Unit>(root);
}
