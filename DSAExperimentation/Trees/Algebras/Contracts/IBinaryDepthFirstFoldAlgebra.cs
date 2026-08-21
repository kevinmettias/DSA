namespace DSAExperimentation.Trees;

public interface IBinaryDepthFirstFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static abstract TResult Combine(
        TNode node,
        BinaryChildren<TResult> children);
}




