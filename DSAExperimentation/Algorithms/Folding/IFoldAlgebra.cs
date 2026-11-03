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
// The children arrive as a span, valid for the one call: the recursive evaluations keep every
// open node's child results on one stack (FoldStack), so a fold allocates no array per node and
// Combine reads them with no interface call. A Combine that needs them later copies them.
//
// Members are instance members on a struct type parameter. A runtime value an algebra
// needs - a label string, a factorial table, a node's coin values - is a field of the
// algebra handed to the fold rather than a static set beforehand: every engine takes
// TAlgebra as a struct, so each algebra is its own JIT instantiation and Combine is still
// a direct, inlinable call (the IVisitGuard precedent). A stateless algebra is passed as
// its default value by each entry point's (root) overload. Pass an algebra by value,
// never through `in` or a readonly field: a defensive copy would drop what its fields
// accumulate, so a counter an algebra must keep lives behind a reference it holds.
/// <summary>
/// What a fold computes: an empty result, and how each node combines with its children's results.
/// </summary>
public interface IFoldAlgebra<TNode, TResult>
{
    /// <summary>The result of folding nothing, which a fold returns for a null root.</summary>
    TResult Empty { get; }

    /// <summary>Combines a node with its children's folded results, as a pure function of those.</summary>
    /// <param name="node">The node being folded.</param>
    /// <param name="children">The folded results of the node's children, in child order; empty for a leaf.</param>
    /// <returns>The node's folded result.</returns>
    TResult Combine(TNode node, ReadOnlySpan<TResult> children);
}
