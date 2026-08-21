namespace DSAExperimentation.Trees;

public static class BinaryDepthFirstFold
{
    public static TResult Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TSchedule : struct, IBinaryChildSchedule
        where TAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            return TAlgebra.Empty;
        }

        var first = Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
            TSchedule.GetFirst<TNode, TTopology>(root));

        var second = Fold<TNode, TTopology, TSchedule, TAlgebra, TResult>(
            TSchedule.GetSecond<TNode, TTopology>(root));

        var children = TSchedule.RestoreSemanticOrder(first, second);

        return TAlgebra.Combine(
            root,
            children);
    }
}




