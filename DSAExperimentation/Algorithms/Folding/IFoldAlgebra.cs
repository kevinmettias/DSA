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
//
// Members are instance members on a struct type parameter. A runtime value an algebra
// needs - a label string, a factorial table, a node's coin values - is a field of the
// algebra handed to the fold rather than a static set beforehand: every engine takes
// TAlgebra as a struct, so each algebra is its own JIT instantiation and Combine is still
// a direct, inlinable call (the IVisitGuard precedent). A stateless algebra is passed as
// its default value by each entry point's (root) overload. Pass an algebra by value,
// never through `in` or a readonly field: a defensive copy would drop what its fields
// accumulate, so a counter an algebra must keep lives behind a reference it holds.
internal interface IFoldAlgebra<TNode, TResult>
{
    TResult Empty { get; }

    TResult Combine(TNode node, IReadOnlyList<TResult> children);
}
