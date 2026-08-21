namespace DSAExperimentation.Trees;

public interface IChildOrder<TNode>
{
    static abstract IEnumerable<TNode> Apply(IEnumerable<TNode> children);
}




