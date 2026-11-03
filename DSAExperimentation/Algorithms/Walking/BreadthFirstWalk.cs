using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Algorithms.Walking;

// The single breadth-first engine behind every BFS-shaped consumer - Reduce through
// BreadthFirstReduceOrder, and BreadthFirstTraversal through Reduce - see
// DepthFirstWalk for the reasoning behind sharing one engine across the tree-only
// and graph-safe cases via TGuard. Walk only ever calls TStep.Enter - BFS has no
// Exit moment (see IBreadthFirstHooks), so Exit is simply never invoked here
// regardless of what TStep does with it.
//
// Walked one level at a time (a List per depth, computed by NextLevel below) rather
// than a Queue<(Node, Depth)>, so that LevelGroupedBreadthFirstTraversal - which
// needs a whole depth buffered before it can fire, since a level isn't complete
// until every node at that depth has been discovered - can share the exact same
// frontier computation instead of reimplementing the enqueue/dequeue mechanics.
// Stepping through a level in list order and then computing the next level from it
// visits nodes in the identical order a FIFO queue would: everything enqueued at
// depth d is dequeued, in order, before anything enqueued at depth d + 1.
//
// A state the algebra calls done stops the walk at once, mid-level if need be, so a search
// for one node stops at the first node of the depth that holds it. Whether to ask is
// TStopCheck, as in DepthFirstWalk.
internal static class BreadthFirstWalk
{
    internal static TState Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
        TNode root, TState state, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : struct, IVisitGuard<TNode>
        where TStep : struct, IReduceAlgebra<TNode, TState>
        where TStopCheck : struct, IStopCheck
        => WalkLevels<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>([root], state, guard);

    // Every root the guard admits is depth 0, so the walk is one breadth-first search from all of
    // them at once: a node's depth is its edge count from the nearest root.
    internal static TState WalkFrom<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
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
        var firstLevel = new List<TNode>();

        foreach (var root in roots)
        {
            if (guard.ShouldVisit(root))
            {
                firstLevel.Add(root);
            }
        }

        return WalkLevels<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(firstLevel, state, guard);
    }

    private static TState WalkLevels<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard, TStep, TState, TStopCheck>(
        List<TNode> firstLevel, TState state, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : struct, IVisitGuard<TNode>
        where TStep : struct, IReduceAlgebra<TNode, TState>
        where TStopCheck : struct, IStopCheck
    {
        var currentLevel = firstLevel;
        var depth = 0;

        while (currentLevel.Count > 0)
        {
            foreach (var node in currentLevel)
            {
                state = TStep.Enter(state, node, depth);

                if (TStopCheck.IsEnabled && TStep.IsDone(state))
                {
                    return state;
                }
            }

            currentLevel = NextLevel<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard>(
                currentLevel, guard);
            depth++;
        }

        return state;
    }

    // The frontier computation shared with LevelGroupedBreadthFirstTraversal: every
    // node in the current level contributes its ordered, guard-filtered children to
    // the next level. This is the one piece either consumer actually needs from a
    // "BFS engine" - what happens to a level once it's known (step through it node
    // by node, or fire once for the whole buffered list) is up to the caller.
    internal static List<TNode> NextLevel<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TGuard>(
        List<TNode> currentLevel, TGuard guard)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TGuard : struct, IVisitGuard<TNode>
    {
        var nextLevel = new List<TNode>();

        foreach (var node in currentLevel)
        {
            var children = TOrder.Apply(TTopology.GetChildren(node));

            for (var i = 0; i < children.Count; i++)
            {
                var child = children.Get(i);

                if (guard.ShouldVisit(child))
                {
                    nextLevel.Add(child);
                }
            }
        }

        return nextLevel;
    }
}
