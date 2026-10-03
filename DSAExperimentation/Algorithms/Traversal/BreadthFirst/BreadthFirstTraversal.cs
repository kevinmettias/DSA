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
internal static class BreadthFirstTraversal
{
    // From the hook's default value - a hook whose fields start empty, such as a counter.
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
    public static THooks Walk<TNode, TTopology, TChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

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
    public static THooks Walk<TNode, TTopology, TChildren, THooks>(TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => Walk<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root, hooks);

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
    public static THooks WalkGraph<TNode, TTopology, TChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, THooks>(root);

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
