namespace DSAExperimentation.Trees;

public interface IBinaryFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static abstract TResult Combine(
        TNode node,
        BinaryChildren<TResult> children);
}




