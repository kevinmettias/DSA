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
internal static class DepthFirstTraversal
{
    // From the hook's default value - a hook whose fields start empty, such as a counter.
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

    public static THooks WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(
        TNode? root, THooks hooks)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(root, hooks, []);

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
        where THooks : struct, IDepthFirstHooks<TNode>
        => Reduce.Graph<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            DepthFirstReduceOrder<TNode>, HooksStep<TNode, THooks>, THooks>(root, hooks, visited);

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
