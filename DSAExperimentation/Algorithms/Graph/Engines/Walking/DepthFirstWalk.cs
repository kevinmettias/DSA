using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Graph.Engines.Walking;
using DSAExperimentation.Algorithms.Graph.Engines.Reducing;
using DSAExperimentation.Algorithms.Graph.Engines.Traversal.DepthFirst;

namespace DSAExperimentation.Algorithms.Graph.Engines.Walking;

// The single depth-first engine behind every DFS-shaped consumer: DepthFirstReduceOrder
// (via the state-threading overload - IReduceAlgebra already has exactly this shape,
// reused by both Reduce.Tree and Reduce.Graph regardless of which guard they pass
// in), and DepthFirstTraversal.Walk/WalkGraph (via the void overload below). A
// tree-only walk and a graph-safe walk differ by
// exactly one thing - whether a child needs to be checked against a visited-set
// before being walked - so that's the only axis exposed as a type parameter
// (TGuard); everything else about the recursion is shared. Constrained on the
// weaker IGraphTopology rather than ITreeTopology since both entry points share
// this engine now; the tree-vs-graph safety gate lives at those entry points
// (which guard they construct), not here. Folding cannot share this: Combine needs
// every child's result at once, as a batch, not one value threaded through
// sequentially, so it keeps its own recursion.
internal static class DepthFirstWalk
{
    internal static TState Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState>(
        TNode node, int depth, TState state, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : IVisitGuard<TNode>
        where TStep : struct, IReduceAlgebra<TNode, TState>
    {
        state = TStep.Enter(state, node, depth);

        var children = TOrder.Apply(TTopology.GetChildren(node));

        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);

            if (guard.ShouldVisit(child))
            {
                state = Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState>(
                    child, depth + 1, state, guard);
            }
        }

        return TStep.Exit(state, node, depth);
    }

    // Unit is the terminal object in the category of types - there's exactly one
    // function from anything into it, so "run a void IDepthFirstHooks pair through
    // the state-threading walk above" has exactly one sensible implementation. That
    // canonical embedding lives here, once, beside the engine it specializes,
    // rather than being rewritten at each consumer that wants void hooks.
    internal static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, THooks>(
        TNode node, int depth, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : IVisitGuard<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, HooksStep<TNode, THooks>, Unit>(
            node, depth, default, guard);

    private readonly struct HooksStep<TNode, THooks> : IReduceAlgebra<TNode, Unit>
        where TNode : class
        where THooks : struct, IDepthFirstHooks<TNode>
    {
        public static Unit Seed => default;

        public static Unit Enter(Unit state, TNode node, int depth)
        {
            THooks.Enter(node, depth);
            return default;
        }

        public static Unit Exit(Unit state, TNode node, int depth)
        {
            THooks.Exit(node, depth);
            return default;
        }
    }
}
