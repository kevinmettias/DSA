using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;
using DSAExperimentation.Graph.Engines.Dags.Trees;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Engines.Traversal.TopDown;

// One entry point per topology tier - see DepthFirstTraversal for the reasoning.
// Depth-first only: there's no BreadthFirst counterpart yet, since every problem
// motivating this primitive (root-to-leaf state) is naturally depth-first, and
// building one speculatively without a real use case would be exactly the kind of
// unjustified plurality this library has been deliberately avoiding.
internal static class TopDownTraversal
{
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
}
