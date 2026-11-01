using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Traversal.TopDown;

// One entry point per topology tier - see Reduce for the reasoning. TopDown keeps
// that gate itself rather than riding Reduce's the way DepthFirstTraversal does: it
// hands each child a state computed from its parent's, which a single threaded
// accumulator cannot express (see ITopDownHooks).
// Depth-first only: there's no BreadthFirst counterpart yet, since every problem
// motivating this primitive (root-to-leaf state) is naturally depth-first, and
// building one speculatively without a real use case would be exactly the kind of
// unjustified plurality this library has been deliberately avoiding.
/// <summary>
/// Depth-first walks whose <see cref="ITopDownHooks{TNode, TState}"/> hand each child a state derived from
/// its parent's.
/// </summary>
public static class TopDownTraversal
{
    /// <summary>
    /// Walks a tree depth-first, visiting each node before its children and entering those in <c>TOrder</c>'s order.
    /// </summary>
    /// <param name="root">The node to start from; null walks nothing.</param>
    /// <param name="initialState">
    /// The root's state, from which <c>THooks.Descend</c> derives every other node's.
    /// </param>
    public static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks, TState>(
        TNode? root, TState initialState)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, ITopDownHooks<TNode, TState>
    {
        if (root is null)
        {
            return;
        }

        TopDownWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>, THooks, TState>(
            root, initialState, 0, default);
    }

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree depth-first, visiting each node before its children and entering those in the topology's own order.
    /// </summary>
    /// <param name="root">The node to start from; null walks nothing.</param>
    /// <param name="initialState">
    /// The root's state, from which <c>THooks.Descend</c> derives every other node's.
    /// </param>
    public static void Walk<TNode, TTopology, TChildren, THooks, TState>(
        TNode? root, TState initialState)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, ITopDownHooks<TNode, TState>
        => Walk<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            THooks, TState>(
            root, initialState);

    /// <summary>
    /// Walks a graph depth-first, visiting each node it reaches once and entering unvisited children in
    /// <c>TOrder</c>'s order.
    /// </summary>
    /// <param name="root">The node to start from; null walks nothing.</param>
    /// <param name="initialState">
    /// The root's state; any other node gets its state along the first path that reaches it.
    /// </param>
    public static void WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks, TState>(
        TNode? root, TState initialState)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, ITopDownHooks<TNode, TState>
    {
        if (root is null)
        {
            return;
        }

        TopDownWalk.Walk<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, THooks, TState>(
            root, initialState, 0, new TrackedVisitGuard<TNode>(new HashSet<TNode> { root }));
    }

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph depth-first, visiting each node it reaches once and entering unvisited children in the
    /// topology's order.
    /// </summary>
    /// <param name="root">The node to start from; null walks nothing.</param>
    /// <param name="initialState">
    /// The root's state; any other node gets its state along the first path that reaches it.
    /// </param>
    public static void WalkGraph<TNode, TTopology, TChildren, THooks, TState>(
        TNode? root, TState initialState)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, ITopDownHooks<TNode, TState>
        => WalkGraph<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            THooks, TState>(
            root, initialState);
}
