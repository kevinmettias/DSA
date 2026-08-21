namespace DSAExperimentation.Trees;

public static class BinaryTreeFold
{
    public static TResult Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAlgebra : struct, IBinaryTreeFoldAlgebra<TNode, TResult>
    {
        if (root is null)
            return TAlgebra.Empty;

        var first = Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
            TSchedule.GetFirst<TNode, TTopology>(root));

        var second = Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
            TSchedule.GetSecond<TNode, TTopology>(root));

        return TAlgebra.Combine(
            root,
            TSchedule.RestoreSemanticOrder(first, second));
    }
}
