using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;
using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Folding.Dags;

// The trusted counterpart to CheckedFold: IDagTopology already promises acyclicity,
// so there's nothing to defend against - no inProgress tracking, no cycle-detection
// throw, just memoization for the (still real, still expected) case of a shared
// descendant. If the promise turns out to be false, this stack-overflows or hangs,
// the same way RecursiveFoldEvaluation would on a non-tree ITreeTopology - CheckedFold
// is the checked fallback for when you can't vouch for acyclicity yourself.
internal static class DagFold
{
    public static TResult Fold<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
        => root is null
            ? TAlgebra.Empty
            : Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
                root, 0, new Dictionary<TNode, TResult>());

    private static TResult Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode node, int depth, Dictionary<TNode, TResult> completed)
        where TNode : class
        where TTopology : struct, IDagTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        if (completed.TryGetValue(node, out var cached))
        {
            return cached;
        }

        TAlgebra.Enter(node, depth);

        var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
        var childResults = new TResult[orderedChildren.Count];

        for (var i = 0; i < orderedChildren.Count; i++)
        {
            childResults[i] = Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
                orderedChildren.Get(i), depth + 1, completed);
        }

        var result = TAlgebra.Combine(node, childResults);
        completed[node] = result;

        return result;
    }
}
