using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Algorithms.Walking;

// The single depth-first engine behind every DFS-shaped consumer: Reduce.Tree and
// Reduce.Graph reach it through DepthFirstReduceOrder, whichever guard they pass in,
// and DepthFirstTraversal reaches it through Reduce, threading its hook as the state.
// A tree-only walk and a graph-safe walk differ by exactly one thing - whether a child needs to be checked against a visited-set
// before being walked - so that's the only axis exposed as a type parameter
// (TGuard); everything else about the recursion is shared. Constrained on the
// weaker IGraphTopology rather than ITreeTopology since both entry points share
// this engine now; the tree-vs-graph safety gate lives at those entry points
// (which guard they construct), not here. Folding cannot share this: Combine needs
// every child's result at once, as a batch, not one value threaded through
// sequentially, so it keeps its own recursion.
//
// A state the algebra calls done stops the walk where it stands: no further child is
// entered and no node still open is exited, the same way at every depth. Whether to ask is
// TStopCheck, a type the caller picks from the algebra's StopsEarly (see IStopCheck), so the
// walk of an algebra that never stops carries no check.
internal static class DepthFirstWalk
{
    // Walks each root the guard admits in turn, over the one guard, so a node two roots reach is
    // walked from the first of them only.
    internal static TState WalkEach<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
        IEnumerable<TNode> roots, TState state, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : struct, IVisitGuard<TNode>
        where TStep : struct, IReduceAlgebra<TNode, TState>
        where TStopCheck : struct, IStopCheck
    {
        foreach (var root in roots)
        {
            if (guard.ShouldVisit(root))
            {
                state = Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(root, 0, state, guard);

                if (TStopCheck.IsEnabled && TStep.IsDone(state))
                {
                    return state;
                }
            }
        }

        return state;
    }

    internal static TState Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
        TNode node, int depth, TState state, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : struct, IVisitGuard<TNode>
        where TStep : struct, IReduceAlgebra<TNode, TState>
        where TStopCheck : struct, IStopCheck
    {
        state = TStep.Enter(state, node, depth);

        if (TStopCheck.IsEnabled && TStep.IsDone(state))
        {
            return state;
        }

        var children = TOrder.Apply(TTopology.GetChildren(node));

        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);

            if (guard.ShouldVisit(child))
            {
                state = Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
                    child, depth + 1, state, guard);

                if (TStopCheck.IsEnabled && TStep.IsDone(state))
                {
                    return state;
                }
            }
        }

        return TStep.Exit(state, node, depth);
    }
}
