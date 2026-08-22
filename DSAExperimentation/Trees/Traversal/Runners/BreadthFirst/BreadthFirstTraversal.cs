namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstTraversal<TNode, TTopology, TOrder, THooks>
    : ITreeTraversal<TNode>
    where TNode : class
    where TTopology : struct, ITreeTopology<TNode>
    where TOrder : struct, IChildOrder<TNode>
    where THooks : struct, IBreadthFirstFoldAlgebra<TNode, Unit>
{
    public static void Traverse(TNode? root)
        => BreadthFirstWalk.Walk<
            TNode,
            ChildOrderStrategy<TNode, TTopology, TOrder>,
            THooks,
            Unit>(root);
}
