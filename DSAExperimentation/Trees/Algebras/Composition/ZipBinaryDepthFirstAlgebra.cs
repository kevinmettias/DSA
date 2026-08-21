namespace DSAExperimentation.Trees;

public readonly struct ZipBinaryDepthFirstAlgebra<
    TNode,
    TFirstResult,
    TSecondResult,
    TFirstAlgebra,
    TSecondAlgebra>
    : IBinaryDepthFirstFoldAlgebra<TNode, FoldResultPair<TFirstResult, TSecondResult>>
    where TFirstAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TFirstResult>
    where TSecondAlgebra : struct, IBinaryDepthFirstFoldAlgebra<TNode, TSecondResult>
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




