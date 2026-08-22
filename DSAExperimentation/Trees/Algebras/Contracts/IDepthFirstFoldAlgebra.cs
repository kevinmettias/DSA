namespace DSAExperimentation.Trees;

public interface IDepthFirstFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static abstract TResult Combine(
        TNode node,
        IReadOnlyList<TResult> children);

    static virtual void Enter(TNode node)
    {
    }

    static virtual bool CollectsChildResults
        => true;
}




