namespace DSAExperimentation.Trees;

public interface IBinaryChildSchedule
{
    static abstract TNode? GetFirst<TNode, TTopology>(TNode node)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>;

    static abstract TNode? GetSecond<TNode, TTopology>(TNode node)
        where TNode : class
        where TTopology : struct, IBinaryTreeTopology<TNode>;

    static abstract BinaryChildren<TResult> RestoreSemanticOrder<TResult>(
        TResult first,
        TResult second);
}
