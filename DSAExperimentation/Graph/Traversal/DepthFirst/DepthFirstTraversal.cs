namespace DSAExperimentation.Graph;

// One entry point per topology tier, not per "tree" vs "graph" naming for its own
// sake: which guard gets wired in is chosen by the compiler, not exposed as a
// caller-visible parameter, so an unguarded walk over a topology that doesn't prove
// unique ancestry is unrepresentable. Walk only compiles against ITreeTopology and
// always uses UnguardedVisit; WalkGraph is the only path into a cyclic/shared
// topology, and it always pays for TrackedVisitGuard. Both ride the same
// DepthFirstWalk engine.
public static class DepthFirstTraversal
{
    public static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
    {
        if (root is null)
        {
            return;
        }

        DepthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>, THooks>(
            root, 0, default);
    }

    public static void WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(root, []);

    // The multi-root overload - see Reduce.Graph's for the reasoning: visited is
    // supplied by the caller so it can be carried across several separate
    // top-level calls, which is what whole-graph algorithms (connected
    // components, multi-source BFS) need and the single-root overload above
    // cannot express on its own.
    public static void WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(
        TNode? root, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IDepthFirstHooks<TNode>
    {
        if (root is null || !visited.Add(root))
        {
            return;
        }

        DepthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, THooks>(
            root, 0, new TrackedVisitGuard<TNode>(visited));
    }
}
