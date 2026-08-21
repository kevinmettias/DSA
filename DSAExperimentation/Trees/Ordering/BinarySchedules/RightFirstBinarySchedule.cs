namespace DSAExperimentation.Trees;

public readonly struct RightFirstBinarySchedule : IBinaryChildSchedule
{
    public static TNode? GetFirst<TNode, TTopology>(TNode node)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        => TTopology.GetRight(node);

    public static TNode? GetSecond<TNode, TTopology>(TNode node)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>
        => TTopology.GetLeft(node);

    public static BinaryChildren<TResult> RestoreSemanticOrder<TResult>(
        TResult first,
        TResult second)
        => new(second, first);
}




