namespace DSAExperimentation.Graph;

// The catamorphism: Combine only ever sees a node together with its already-folded
// children, never anything about visit order or timing. That's what makes a fold's
// result independent of *how* you choose to compute it - unlike a reduce, which
// threads a single accumulator through a chosen linearization and is genuinely
// sensitive to that choice.
public interface IFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static virtual void Enter(TNode node, int depth)
    {
    }

    static abstract TResult Combine(TNode node, IReadOnlyList<TResult> children);
}
