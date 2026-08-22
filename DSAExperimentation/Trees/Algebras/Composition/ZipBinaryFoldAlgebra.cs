namespace DSAExperimentation.Trees;

public readonly struct ZipBinaryFoldAlgebra<
    TNode,
    TFirstResult,
    TSecondResult,
    TFirstAlgebra,
    TSecondAlgebra>
    : IBinaryFoldAlgebra<TNode, FoldResultPair<TFirstResult, TSecondResult>>
    where TFirstAlgebra : struct, IBinaryFoldAlgebra<TNode, TFirstResult>
    where TSecondAlgebra : struct, IBinaryFoldAlgebra<TNode, TSecondResult>
{
    public static FoldResultPair<TFirstResult, TSecondResult> Empty
        => new(TFirstAlgebra.Empty, TSecondAlgebra.Empty);

    public static FoldResultPair<TFirstResult, TSecondResult> Combine(
        TNode node,
        BinaryChildren<FoldResultPair<TFirstResult, TSecondResult>> children)
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




