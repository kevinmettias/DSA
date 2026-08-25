namespace DSAExperimentation.Algorithms.Reducing;

// The IReduceAlgebra counterpart to ZipFoldAlgebra: runs two reduce algebras
// together in one traversal by threading a paired state through both, made possible
// the same way - TState was never assumed to be a scalar, so a tuple of two states
// is just another TState.
internal readonly struct ZipReduceAlgebra<TNode, TStateA, TStateB, TAlgebraA, TAlgebraB>
    : IReduceAlgebra<TNode, (TStateA A, TStateB B)>
    where TAlgebraA : struct, IReduceAlgebra<TNode, TStateA>
    where TAlgebraB : struct, IReduceAlgebra<TNode, TStateB>
{
    public static (TStateA A, TStateB B) Seed
        => (TAlgebraA.Seed, TAlgebraB.Seed);

    public static (TStateA A, TStateB B) Enter((TStateA A, TStateB B) state, TNode node, int depth)
        => (TAlgebraA.Enter(state.A, node, depth), TAlgebraB.Enter(state.B, node, depth));

    public static (TStateA A, TStateB B) Exit((TStateA A, TStateB B) state, TNode node, int depth)
        => (TAlgebraA.Exit(state.A, node, depth), TAlgebraB.Exit(state.B, node, depth));
}
