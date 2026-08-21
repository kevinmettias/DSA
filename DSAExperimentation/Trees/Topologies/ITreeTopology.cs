namespace DSAExperimentation.Trees;

public interface ITreeTopology<TNode>
    where TNode : class
{
    static abstract IEnumerable<TNode> GetChildren(TNode node);
}




