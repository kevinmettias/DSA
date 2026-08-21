namespace DSAExperimentation.Trees;

public interface IBinaryTreeTopology<TNode> : ITreeTopology<TNode>
    where TNode : class
{
    static abstract TNode? GetLeft(TNode node);

    static abstract TNode? GetRight(TNode node);
}




