namespace DSAExperimentation.Graph;

// One entry point per topology tier - see DepthFirstTraversal for the reasoning:
// Walk only compiles against ITreeTopology and always uses UnguardedVisit;
// WalkGraph is the only path into a cyclic/shared topology, and it always pays for
// TrackedVisitGuard. Both ride the same BreadthFirstWalk engine.
public static class BreadthFirstTraversal
{
    public static void Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        if (root is null)
        {
            return;
        }

        BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>, THooks>(
            root, default);
    }

    public static void WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
        => WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(root, []);

    // The multi-root overload - see Reduce.Graph's for the reasoning.
    public static void WalkGraph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, THooks>(
        TNode? root, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where THooks : struct, IBreadthFirstHooks<TNode>
    {
        if (root is null || !visited.Add(root))
        {
            return;
        }

        BreadthFirstWalk.Walk<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, THooks>(
            root, new TrackedVisitGuard<TNode>(visited));
    }
}
