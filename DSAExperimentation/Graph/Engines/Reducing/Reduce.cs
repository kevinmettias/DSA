using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;
using DSAExperimentation.Graph.Engines.Dags.Trees;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Engines.Reducing;

// One entry point per topology tier - mirroring DepthFirstTraversal/
// BreadthFirstTraversal's Walk/WalkGraph split: which guard gets wired in is chosen
// by the compiler, not exposed as a caller-visible parameter, so an unguarded
// reduce over a topology that doesn't prove unique ancestry is unrepresentable. Tree
// only compiles against ITreeTopology and always uses UnguardedVisit; Graph is the
// only path into a cyclic/shared topology, and it always pays for
// TrackedVisitGuard.
//
// Traversal order is the orthogonal axis, injected as a TOrderStrategy (see
// IReduceOrderStrategy) exactly the way TreeFold injects TStrategy - no default
// here (unlike TreeFold.Fold's recursive default), since order can change a
// reduce's result and callers must choose deliberately.
internal static class Reduce
{
    public static TState Tree<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => root is null
            ? TAlgebra.Seed
            : TOrderStrategy.Evaluate<
                TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>, TAlgebra, TState>(
                root, default);

    public static TState Graph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => Graph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
            root, []);

    // The multi-root overload: visited is supplied by the caller instead of
    // constructed fresh, so it can be carried across several separate top-level
    // calls - what a whole-graph algorithm (counting connected components,
    // multi-source BFS) needs and the single-root overload above structurally
    // cannot express, since that one owns its guard's backing set for the
    // duration of exactly one call. A root already present in visited is treated
    // as nothing to do (TAlgebra.Seed, no walk) - the caller's job is deciding
    // whether that's expected (see ConnectedComponents, which checks first and
    // skips instead of calling this at all) or not.
    public static TState Graph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => root is null || !visited.Add(root)
            ? TAlgebra.Seed
            : TOrderStrategy.Evaluate<
                TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, TAlgebra, TState>(
                root, new TrackedVisitGuard<TNode>(visited));
}
