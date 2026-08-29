using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Folding;

// Fold generalized to an untrusted IGraphTopology: memoizes each node's result (so a
// shared descendant is combined once, not once per incoming path) and detects true
// cycles - a node still on the current recursion path being reached again. A cycle is
// this fold's own reason to exist rather than a caller bug (see IFoldAlgebra's
// purity/well-foundedness note) - the caller reached for IGraphTopology specifically
// because it couldn't vouch for acyclicity itself, so TryFold reports a cycle as
// Success: false rather than throwing. No throwing Fold convenience: whether the
// structure is cyclic is exactly the thing an external caller reached for this type
// to find out, so it can't be presumed away the way an empty-container check can -
// TryFold is the only public surface for it. See DagFold for the trusted counterpart
// that skips the cycle defense entirely once IDagTopology promises it's unnecessary.
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
    {
        if (root is null)
        {
            result = TAlgebra.Empty;
            return true;
        }

        var outcome = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
            root, 0, new Dictionary<TNode, TResult>(), new HashSet<TNode>());

        result = outcome.Result;
        return outcome.Success;
    }

    private static VisitOutcome<TResult> Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode node, int depth, Dictionary<TNode, TResult> completed, HashSet<TNode> inProgress)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (completed.TryGetValue(node, out var cached))
        {
            return new VisitOutcome<TResult>(true, cached);
        }

        return inProgress.Add(node)
            ? Descend<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(node, depth, completed, inProgress)
            : VisitOutcome<TResult>.Failure;
    }

    // The per-node work once Visit has ruled out both a memoized result and a cycle:
    // enter, fold the children, combine, and memoize.
    private static VisitOutcome<TResult> Descend<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode node, int depth, Dictionary<TNode, TResult> completed, HashSet<TNode> inProgress)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        TAlgebra.Enter(node, depth);

        var children = CollectChildResults<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
            node, depth, completed, inProgress);

        if (!children.Success)
        {
            return VisitOutcome<TResult>.Failure;
        }

        var result = TAlgebra.Combine(node, children.Result);

        return Complete(node, result, completed, inProgress);
    }

    private static VisitOutcome<TResult> Complete<TNode, TResult>(
        TNode node, TResult result, Dictionary<TNode, TResult> completed, HashSet<TNode> inProgress)
        where TNode : class
    {
        inProgress.Remove(node);
        completed[node] = result;

        return new VisitOutcome<TResult>(true, result);
    }

    private static VisitOutcome<TResult[]> CollectChildResults<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode node, int depth, Dictionary<TNode, TResult> completed, HashSet<TNode> inProgress)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
        var childResults = new TResult[orderedChildren.Count];

        for (var i = 0; i < orderedChildren.Count; i++)
        {
            var child = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
                orderedChildren.Get(i), depth + 1, completed, inProgress);

            if (!child.Success)
            {
                return VisitOutcome<TResult[]>.Failure;
            }

            childResults[i] = child.Result;
        }

        return new VisitOutcome<TResult[]>(true, childResults);
    }

    // Success is false exactly when a cycle was found beneath this call; Result is
    // meaningful only when Success is true, the same convention TryX(out T) uses
    // everywhere else in this library.
    private readonly record struct VisitOutcome<TValue>(bool Success, TValue Result)
    {
        // presumption: allow -- Result is only meaningful when Success is true, the
        // standard TryGetValue/TryParse out-parameter contract this mirrors.
        public static VisitOutcome<TValue> Failure => new(false, default!);
    }
}
