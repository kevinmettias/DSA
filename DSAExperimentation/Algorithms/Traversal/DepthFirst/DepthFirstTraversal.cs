using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Algorithms.Traversal.DepthFirst;

// A depth-first traversal is a depth-first reduce whose state is the hook itself:
// every entry point threads the hook value through Reduce as TState, HooksStep turns
// each reduce step into the hook's Enter or Exit, and the walk returns the hook value
// it finished with. So the tier gate is Reduce's - Walk only compiles against
// ITreeTopology and never pays for a visited set; WalkGraph is the only path into a
// cyclic/shared topology, and it always tracks one - and so is the engine. What stays
// distinct is the concept: a traversal observes nodes through a hook, a reduce
// accumulates an answer through an algebra.
/// <summary>Depth-first walks over a tree or a graph that report each node's entry and exit to a hook.</summary>
public static class DepthFirstTraversal
{
    // From the hook's default value - a hook whose fields start empty, such as a counter.
    /// <summary>
    /// Walks a tree depth-first in <c>TOrder</c>'s child order, starting from the hook's default value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Reduce.Tree<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DepthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root);

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree depth-first in the topology's own child order, starting from the hook's default value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

    /// <summary>
    /// Walks a tree depth-first in <c>TOrder</c>'s child order, starting from the given hook value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Reduce.Tree<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DepthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root, hooks);

    // In the topology's own child order.
    /// <summary>
    /// Walks a tree depth-first in the topology's own child order, starting from the given hook value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);

    /// <summary>
    /// Walks a graph depth-first, each reachable node once, in <c>TOrder</c>'s child order, from the hook's
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DepthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph depth-first, each reachable node once, in the topology's own child order, from the
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

    /// <summary>
    /// Walks a graph depth-first, each reachable node once, in <c>TOrder</c>'s child order, from the given hook value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(root, hooks, []);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph depth-first, each reachable node once, in the topology's own child order, from the given
    /// hook value.
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);

    // The multi-root overload - see Reduce.Graph's for the reasoning: visited is
    // supplied by the caller so it can be carried across several separate
    // top-level calls, which is what whole-graph algorithms (connected
    // components, multi-source BFS) need and the single-root overloads above
    // cannot express on their own. visited comes last and never without the hooks,
    // as in Reduce.
    /// <summary>
    /// Walks a graph depth-first in <c>TOrder</c>'s child order, skipping and recording nodes in a visited
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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DepthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root, hooks, visited);

    // In the topology's own child order.
    /// <summary>
    /// Walks a graph depth-first in the topology's own child order, skipping and recording nodes in a visited
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
    public static THooks WalkGraph<TNode, TTopology, TChildren, THooks>(
        TNode? root, THooks hooks, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks, visited);

    // The reduce step a hook rides as: the hook value is the state, so each event runs on
    // the threaded copy and passes it on. Seed is where a walk with no hook of its own
    // starts - the (root) overloads read it through Reduce's.
    private readonly struct HooksStep<TNode, THooks> : IReduceAlgebra<TNode, THooks>
        where THooks : struct, IDepthFirstHooks<TNode>
    {
        public static THooks Seed => default;

        public static THooks Enter(THooks hooks, TNode node, int depth)
        {
            hooks.Enter(node, depth);
            return hooks;
        }

        public static THooks Exit(THooks hooks, TNode node, int depth)
        {
            hooks.Exit(node, depth);
            return hooks;
        }
    }
}
