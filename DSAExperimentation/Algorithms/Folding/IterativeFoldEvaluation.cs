using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Algorithms.Folding;

// Discovers every node breadth-first (an explicit queue, not the call stack), then
// combines bottom-up in reverse discovery order. The breadth-first discovery is
// just the mechanism - what this buys you is that it won't stack-overflow on a
// deep, unbalanced tree the way RecursiveFoldEvaluation's recursion can. Per the
// purity caveat on IFoldAlgebra, it produces exactly the same result as
// RecursiveFoldEvaluation for any pure algebra; it's a different way to compute the
// same fold, not a different traversal order in any way a caller can observe.
internal readonly struct IterativeFoldEvaluation<TNode> : IFoldEvaluationStrategy<TNode>
    where TNode : class
{
    public static TResult Evaluate<TTopology, TChildren, TOrder, TOrderedChildren, TAlgebra, TResult>(
        TNode root, TAlgebra algebra)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var state = new DiscoveryState();

        DiscoverNodes<TTopology, TChildren, TOrder, TOrderedChildren>(root, state);

        return CombineBottomUp<TAlgebra, TResult>(root, state, algebra);
    }

    private static void DiscoverNodes<TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode root, DiscoveryState state)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        state.Order.Add(root);
        state.Pending.Enqueue(root);

        while (state.Pending.Count > 0)
        {
            DiscoverChildren<TTopology, TChildren, TOrder, TOrderedChildren>(state.Pending.Dequeue(), state);
        }
    }

    private static void DiscoverChildren<TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, DiscoveryState state)
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var orderedChildren = TOrder.Apply(TTopology.GetChildren(node));
        var children = new List<TNode>(orderedChildren.Count);

        for (var i = 0; i < orderedChildren.Count; i++)
        {
            var child = orderedChildren.Get(i);
            state.Order.Add(child);
            children.Add(child);
            state.Pending.Enqueue(child);
        }

        state.ChildrenByNode[node] = children;
    }

    private static TResult CombineBottomUp<TAlgebra, TResult>(TNode root, DiscoveryState state, TAlgebra algebra)
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>
    {
        var results = new Dictionary<TNode, TResult>();

        for (var i = state.Order.Count - 1; i >= 0; i--)
        {
            var node = state.Order[i];
            var children = state.ChildrenByNode[node];
            var childResults = new TResult[children.Count];

            for (var j = 0; j < children.Count; j++)
            {
                childResults[j] = results[children[j]];
            }

            results[node] = algebra.Combine(node, childResults);
        }

        return results[root];
    }

    // Bundles the three collections one breadth-first evaluation thread through, so a
    // traversal step names one state parameter instead of the three collections that make it up.
    private sealed record DiscoveryState
    {
        public Queue<TNode> Pending { get; } = new();
        public List<TNode> Order { get; } = new();
        public Dictionary<TNode, List<TNode>> ChildrenByNode { get; } = new();
    }
}
