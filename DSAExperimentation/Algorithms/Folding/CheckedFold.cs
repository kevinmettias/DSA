using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Folding;

// Fold generalized to an untrusted IGraphTopology: memoizes each node's result (so a
// shared descendant is combined once, not once per incoming path) and detects true
// cycles - a node still on the current recursion path being reached again. A cycle is
// this fold's own reason to exist rather than a caller bug (see IFoldAlgebra's
// purity/well-foundedness note) - the caller reached for IGraphTopology specifically
// because it couldn't vouch for acyclicity itself, so TryFold reports a cycle as
// false rather than throwing. No throwing Fold convenience: whether the structure is
// cyclic is exactly the thing an external caller reached for this type to find out,
// so it can't be presumed away the way an empty-container check can - TryFold is the
// only public surface for it. See DagFold for the trusted counterpart that skips the
// cycle defense entirely once IDagTopology promises it's unnecessary.
//
// The recursion itself is FoldRecursion's, shared with the DAG tier; what
// makes this the graph tier is the policy it hands that recursion, CycleCheckedFold.
// A cycle aborts the walk at once - no later sibling is visited and nothing on the
// cyclic path is combined - and the result is meaningful only when TryFold returns true,
// the TryGetValue/TryParse out-parameter convention.
//
// Deliberately recursive-only, no separate evaluation strategy the way TreeFold
// has: an iterative, stack-safe DAG fold needs a real topological sort. Reverse
// BFS-discovery order - what IterativeFoldEvaluation relies on - is only a valid
// combine order for trees; a DAG edge can skip levels and break it. That's a
// genuine follow-up, not something to fold in here.
internal static class CheckedFold
{
    public static bool TryFold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root, out TResult result)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => TryFold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(root, default, out result);

    // In the topology's own child order.
    public static bool TryFold<TNode, TTopology, TChildren, TAlgebra, TResult>(
        TNode? root, out TResult result)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => TryFold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TAlgebra, TResult>(
            root, out result);

    public static bool TryFold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra, out TResult result)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            result = algebra.Empty;
            return true;
        }

        var memo = new CycleCheckedFold<TNode, TResult>();

        result = FoldRecursion.Visit<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren, CycleCheckedFold<TNode, TResult>, TAlgebra, TResult>(
            root, memo, algebra);

        return !memo.Aborted;
    }

    // In the topology's own child order.
    public static bool TryFold<TNode, TTopology, TChildren, TAlgebra, TResult>(
        TNode? root, TAlgebra algebra, out TResult result)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => TryFold<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TAlgebra, TResult>(
            root, algebra, out result);
}
