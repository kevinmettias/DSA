namespace DSAExperimentation.Trees;

public interface ITreeFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static abstract TResult Combine(
        TNode node,
        IReadOnlyList<TResult> children);
}
