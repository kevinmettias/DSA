namespace DSAExperimentation.Trees;

public readonly struct ZipFoldAlgebra<
    TNode,
    TFirstResult,
    TSecondResult,
    TFirstAlgebra,
    TSecondAlgebra>
    : IFoldAlgebra<TNode, FoldResultPair<TFirstResult, TSecondResult>>
    where TFirstAlgebra : struct, IFoldAlgebra<TNode, TFirstResult>
    where TSecondAlgebra : struct, IFoldAlgebra<TNode, TSecondResult>
{
    public static FoldResultPair<TFirstResult, TSecondResult> Empty
        => new(TFirstAlgebra.Empty, TSecondAlgebra.Empty);

    public static bool CollectsChildResults
        => TFirstAlgebra.CollectsChildResults || TSecondAlgebra.CollectsChildResults;

    public static void Enter(TNode node, int depth)
    {
        TFirstAlgebra.Enter(node, depth);
        TSecondAlgebra.Enter(node, depth);
    }

    public static FoldResultPair<TFirstResult, TSecondResult> Combine(
        TNode node,
        IReadOnlyList<FoldResultPair<TFirstResult, TSecondResult>> children)
    {
        var first = TFirstAlgebra.Combine(node, FirstResults(children));
        var second = TSecondAlgebra.Combine(node, SecondResults(children));
        return new(first, second);
    }

    private static IReadOnlyList<TFirstResult> FirstResults(
        IReadOnlyList<FoldResultPair<TFirstResult, TSecondResult>> children)
    {
        if (!TFirstAlgebra.CollectsChildResults || children.Count == 0)
        {
            return [];
        }

        var results = new TFirstResult[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            results[i] = children[i].First;
        }

        return results;
    }

    private static IReadOnlyList<TSecondResult> SecondResults(
        IReadOnlyList<FoldResultPair<TFirstResult, TSecondResult>> children)
    {
        if (!TSecondAlgebra.CollectsChildResults || children.Count == 0)
        {
            return [];
        }

        var results = new TSecondResult[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            results[i] = children[i].Second;
        }

        return results;
    }
}
