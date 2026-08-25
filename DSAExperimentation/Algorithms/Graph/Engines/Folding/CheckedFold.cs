using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Graph.Engines.Folding;

// Fold generalized to an untrusted IGraphTopology: memoizes each node's result (so a
// shared descendant is combined once, not once per incoming path) and detects true
// cycles - a node still on the current recursion path being reached again - and
// throws, since a catamorphism has no defined meaning there (see IFoldAlgebra's
// purity/well-foundedness note). This is the checked fallback for when you can't
// vouch for acyclicity yourself; see DagFold for the trusted counterpart that skips
// the cycle defense once IDagTopology promises it's unnecessary.
//
// Deliberately recursive-only, no separate evaluation strategy the way TreeFold
// has: an iterative, stack-safe DAG fold needs a real topological sort. Reverse
// BFS-discovery order - what IterativeFoldEvaluation relies on - is only a valid
// combine order for trees; a DAG edge can skip levels and break it. That's a
// genuine follow-up, not something to fold in here.
internal static class CheckedFold
{
    private const string CyclicStructureMessage =
        "CheckedFold requires an acyclic structure - reached a node that is still being folded.";

    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (root is null)
        {
            return TAlgebra.Empty;
        }

        var completed = new Dictionary<TNode, TResult>();
        var inProgress = new HashSet<TNode>();

        return Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
            root, 0, completed, inProgress);
    }

    private static TResult Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
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
            return cached;
        }

        EnterNode(node, inProgress);
        TAlgebra.Enter(node, depth);

        var childResults = CollectChildResults<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
            node, depth, completed, inProgress);
        var result = TAlgebra.Combine(node, childResults);

        inProgress.Remove(node);
        completed[node] = result;

        return result;
    }

    private static void EnterNode<TNode>(TNode node, HashSet<TNode> inProgress)
        where TNode : class
    {
        if (!inProgress.Add(node))
        {
            throw new InvalidOperationException(CyclicStructureMessage);
        }
    }

    private static TResult[] CollectChildResults<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
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
            childResults[i] = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
                orderedChildren.Get(i), depth + 1, completed, inProgress);
        }

        return childResults;
    }
}
