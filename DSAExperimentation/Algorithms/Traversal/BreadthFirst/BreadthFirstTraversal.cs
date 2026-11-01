using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Algorithms.Traversal.BreadthFirst;

// The breadth-first counterpart to DepthFirstTraversal, built the same way: a
// breadth-first reduce whose state is the hook, so the tier gate and the engine are
// Reduce's - Walk only compiles against ITreeTopology and never pays for a visited
// set; WalkGraph is the only path into a cyclic/shared topology, and it always tracks
// one. Breadth-first reduce never calls Exit, so HooksStep maps Enter to the hook's
// Visit alone.
/// <summary>Breadth-first walks over a tree or a graph that report each node and its depth to a hook.</summary>
public static class BreadthFirstTraversal
{
    // From the hook's default value - a hook whose fields start empty, such as a counter.
    /// <summary>
    /// Walks a tree breadth-first in <c>TOrder</c>'s child order, starting from the hook's default value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <returns>
    /// The hook value the walk finished with, or the default hook when <paramref name="root"/> is
    /// <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Reduce.Tree<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            BreadthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root);

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree breadth-first in the topology's own child order, starting from the hook's default value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <returns>
    /// The hook value the walk finished with, or the default hook when <paramref name="root"/> is
    /// <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

    /// <summary>
    /// Walks a tree breadth-first in <c>TOrder</c>'s child order, starting from the given hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Reduce.Tree<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            BreadthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root, hooks);

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree breadth-first in the topology's own child order, starting from the given hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> for an empty tree.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks Walk<TNode, TTopology, TChildren, THooks>(TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);

    /// <summary>
    /// Walks a graph breadth-first, each reachable node once, in <c>TOrder</c>'s child order, from the hook's
    /// default value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <returns>
    /// The hook value the walk finished with, or the default hook when <paramref name="root"/> is
    /// <c>null</c>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            BreadthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph breadth-first, each reachable node once, in the topology's own child order, from the
    /// hook's default value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <returns>
    /// The hook value the walk finished with, or the default hook when <paramref name="root"/> is
    /// <c>null</c>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

    /// <summary>
    /// Walks a graph breadth-first, each reachable node once, in <c>TOrder</c>'s child order, from the given
    /// hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(
        TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(root, hooks, []);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph breadth-first, each reachable node once, in the topology's own child order, from the
    /// given hook value.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, THooks>(
        TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);

    // The multi-root overload - see Reduce.Graph's for the reasoning: visited is
    // supplied by the caller so it can be carried across several separate
    // top-level calls, which is what whole-graph algorithms (connected
    // components, multi-source BFS) need and the single-root overloads above
    // cannot express on their own. visited comes last and never without the hooks,
    // as in Reduce.
    /// <summary>
    /// Walks a graph breadth-first in <c>TOrder</c>'s child order, skipping and recording nodes in a visited
    /// set shared across walks.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <param name="visited">
    /// The nodes already walked, which this walk skips and to which it adds every node it visits.
    /// </param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c> or already in <paramref name="visited"/>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(
        TNode? root, THooks hooks, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            BreadthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root, hooks, visited);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph breadth-first in the topology's own child order, skipping and recording nodes in a
    /// visited set shared across walks.
    /// </summary>
    /// <param name="root">The node to start from, or <c>null</c> to walk nothing.</param>
    /// <param name="hooks">The hook value the walk starts from.</param>
    /// <param name="visited">
    /// The nodes already walked, which this walk skips and to which it adds every node it visits.
    /// </param>
    /// <returns>
    /// The hook value the walk finished with, or <paramref name="hooks"/> unchanged when <paramref
    /// name="root"/> is <c>null</c> or already in <paramref name="visited"/>.
    /// </returns>
    public static THooks WalkGraph<TNode, TTopology, TChildren, THooks>(
        TNode? root, THooks hooks, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks, visited);

    // The reduce step a hook rides as: the hook value is the state, so each event runs on
    // the threaded copy and passes it on. Seed is where a walk with no hook of its own
    // starts - the (root) overloads read it through Reduce's.
    private readonly struct HooksStep<TNode, THooks> : IReduceAlgebra<TNode, THooks>
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        public static THooks Seed => default;

        public static THooks Enter(THooks hooks, TNode node, int depth)
        {
            hooks.Visit(node, depth);
            return hooks;
        }
    }
}
