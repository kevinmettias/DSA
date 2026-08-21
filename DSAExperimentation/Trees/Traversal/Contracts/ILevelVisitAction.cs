namespace DSAExperimentation.Trees;

public interface ILevelVisitAction<TNode>
{
    static abstract void Invoke(IReadOnlyList<TNode> level, int depth);
}




