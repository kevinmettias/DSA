namespace DSAExperimentation.Trees;

public readonly struct ZipBreadthFirstReduceAlgebra<
    TNode,
    TFirstState,
    TSecondState,
    TFirstAlgebra,
    TSecondAlgebra>
    : IBreadthFirstReduceAlgebra<TNode, FoldResultPair<TFirstState, TSecondState>>
    where TFirstAlgebra : struct, IBreadthFirstReduceAlgebra<TNode, TFirstState>
    where TSecondAlgebra : struct, IBreadthFirstReduceAlgebra<TNode, TSecondState>
{
    public static FoldResultPair<TFirstState, TSecondState> Seed
        => new(TFirstAlgebra.Seed, TSecondAlgebra.Seed);

    public static void Discover(TNode node, int depth)
    {
        TFirstAlgebra.Discover(node, depth);
        TSecondAlgebra.Discover(node, depth);
    }

    public static FoldResultPair<TFirstState, TSecondState> Accumulate(
        FoldResultPair<TFirstState, TSecondState> state,
        TNode node,
        int depth)
    {
        var first = TFirstAlgebra.Accumulate(state.First, node, depth);
        var second = TSecondAlgebra.Accumulate(state.Second, node, depth);
        return new(first, second);
    }

    public static void Finish()
    {
        TFirstAlgebra.Finish();
        TSecondAlgebra.Finish();
    }
}
