namespace DSAExperimentation.Trees;

public static class BinaryDepthFirstFold
{
    public static TResult Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TResult>
        => DepthFirstFold.Fold<
            TNode,
            BinaryChildScheduleFoldStrategy<TNode, TTopology, TSchedule, TAlgebra, TResult>,
            TResult>(root);
}
