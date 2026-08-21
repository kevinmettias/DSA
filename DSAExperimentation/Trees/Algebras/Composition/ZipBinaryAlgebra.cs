namespace DSAExperimentation.Trees;

public readonly struct ZipBinaryAlgebra<
    TNode,
    TFirstResult,
    TSecondResult,
    TFirstAlgebra,
    TSecondAlgebra>
    : IBinaryTreeFoldAlgebra<TNode, FoldPair<TFirstResult, TSecondResult>>
    where TFirstAlgebra : struct, IBinaryTreeFoldAlgebra<TNode, TFirstResult>
    where TSecondAlgebra : struct, IBinaryTreeFoldAlgebra<TNode, TSecondResult>
{
    public static FoldPair<TFirstResult, TSecondResult> Empty
        => new(TFirstAlgebra.Empty, TSecondAlgebra.Empty);

    public static FoldPair<TFirstResult, TSecondResult> Combine(
        TNode node,
        BinaryChildren<FoldPair<TFirstResult, TSecondResult>> children)
    {
        var first = TFirstAlgebra.Combine(
            node,
            new BinaryChildren<TFirstResult>(
                children.Left.First,
                children.Right.First));

        var second = TSecondAlgebra.Combine(
            node,
            new BinaryChildren<TSecondResult>(
                children.Left.Second,
                children.Right.Second));

        return new(first, second);
    }
}
