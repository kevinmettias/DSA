using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Folding;

// The one recursion every recursive fold tier runs: recall a finished node, open it, fold its
// children in TOrder, Combine, close it. The tiers differ only in TMemo - what must be remembered
// about nodes already met - so that is the only axis exposed, the way the walk engines expose only
// TGuard. Constrained on the weakest IGraphTopology because all three tiers share it; the tier gate
// stays at the entry points (RecursiveFoldEvaluation for trees, DagFold, CheckedFold), each choosing
// the policy its topology bound has earned, so a tree can never be folded without the promise that
// makes skipping the memo safe.
//
// It returns TResult rather than a success flag: the JIT never inlines a recursion into itself, so a
// bool/out signature would tax every tree-fold call. A cycle is reported through the policy
// instead. Aborted is a constant false for the tree and DAG policies, so its check after each child
// folds away; under CycleCheckedFold the walk unwinds at once and later siblings are never visited.
// An aborted walk returns the algebra's Empty, which means nothing there - CheckedFold reads failure
// from Aborted - but is a real TResult, so no null is ever promised away.
//
// One method, deliberately, though it reads as four steps: each helper split out of it would be
// another frame per level of the recursion. The old CheckedFold spent three frames per level and
// overflowed the stack on a 3,000-node chain; this runs one.
internal static class FoldRecursion
{
    internal static TResult Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TMemo, TAlgebra, TResult>(
        TNode node, TMemo memo, TAlgebra algebra)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TMemo : struct, IFoldMemo<TNode, TResult>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (memo.TryRecall(node, out var recalled))
        {
            return recalled;
        }

        if (!memo.TryOpen(node))
        {
            return algebra.Empty;
        }

        var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
        var childResults = new TResult[orderedChildren.Count];

        for (var i = 0; i < orderedChildren.Count; i++)
        {
            childResults[i] = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TMemo, TAlgebra, TResult>(
                orderedChildren.Get(i), memo, algebra);

            if (memo.Aborted)
            {
                return algebra.Empty;
            }
        }

        var result = algebra.Combine(node, childResults);
        memo.Close(node, result);

        return result;
    }
}
