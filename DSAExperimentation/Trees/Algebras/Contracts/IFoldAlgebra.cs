namespace DSAExperimentation.Trees;

public interface IFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }
    static virtual bool CollectsChildResults
        => true;

    static virtual void Enter(TNode node, int depth)
    {
    }

    static abstract TResult Combine(
        TNode node,
        IReadOnlyList<TResult> children);
}




