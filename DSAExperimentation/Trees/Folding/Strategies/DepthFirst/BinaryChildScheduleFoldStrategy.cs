namespace DSAExperimentation.Trees;

internal readonly struct BinaryChildScheduleFoldStrategy<TNode, TTopology, TSchedule, TAlgebra, TResult>
    : IFoldStrategy<TNode, TResult>
    where TNode : class
    where TTopology : struct, IBinaryTreeTopology<TNode>
    where TSchedule : struct, IBinaryChildSchedule
    where TAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TResult>
{
    public static TResult Empty => TAlgebra.Empty;

    public static TResult FoldNode(TNode node)
    {
        var first = DepthFirstFold.Fold<
            TNode,
            BinaryChildScheduleFoldStrategy<TNode, TTopology, TSchedule, TAlgebra, TResult>,
            TResult>(TSchedule.GetFirst<TNode, TTopology>(node));

        var second = DepthFirstFold.Fold<
            TNode,
            BinaryChildScheduleFoldStrategy<TNode, TTopology, TSchedule, TAlgebra, TResult>,
            TResult>(TSchedule.GetSecond<TNode, TTopology>(node));

        var children = TSchedule.RestoreSemanticOrder(first, second);

        return TAlgebra.Combine(
            node,
            children);
    }
}
