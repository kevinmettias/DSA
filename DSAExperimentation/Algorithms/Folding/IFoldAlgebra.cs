namespace DSAExperimentation.Algorithms.Folding;

// The catamorphism: Combine only ever sees a node together with its already-folded
// children, never anything about visit order or timing. That's what makes a fold's
// result independent of *how* you choose to compute it - unlike a reduce, which
// threads a single accumulator through a chosen linearization and is genuinely
// sensitive to that choice.
//
// Combine must be pure - its only inputs the node and its children's results - and
// that is the whole contract: there is no visit hook, so no evaluation strategy and no
// fold tier can expose an order of its own to an algebra. RecursiveFoldEvaluation and
// IterativeFoldEvaluation call Combine in different orders (post-order, reverse
// breadth-first), which only an impure Combine could notice.
internal interface IFoldAlgebra<TNode, TResult>
{
    static abstract TResult Empty { get; }

    static abstract TResult Combine(TNode node, IReadOnlyList<TResult> children);
}
