using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

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
        => Tree<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(root, TAlgebra.Seed);

    // An explicit starting state, for a reduce whose seed belongs to the call rather than to the
    // algebra alone - a traversal threading its own hook value through the walk is one. An empty
    // walk answers with that seed, not TAlgebra.Seed: the caller's starting state is what nothing
    // was folded into.
    public static TState Tree<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root, TState seed)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => root is null
            ? seed
            : TOrderStrategy.Evaluate<
                TTopology, TChildren, TOrder, TOrderedChildren, UnguardedVisit<TNode>, TAlgebra, TState>(
                root, seed, default);

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
            root, TAlgebra.Seed, []);

    // The multi-root overload: visited is supplied by the caller instead of
    // constructed fresh, so it can be carried across several separate top-level
    // calls - what a whole-graph algorithm (counting connected components,
    // multi-source BFS) needs and the single-root overload above structurally
    // cannot express, since that one owns its guard's backing set for the
    // duration of exactly one call. A root already present in visited is treated
    // as nothing to do (the seed, no walk) - the caller's job is deciding
    // whether that's expected (see ConnectedComponents, which checks first and
    // skips instead of calling this at all) or not.
    //
    // visited always comes last and never without a seed. A (root, visited) overload
    // beside a (root, seed) one would let C#'s tie-break bind a HashSet<TNode> seed as
    // the visited set - silently, not as an ambiguity error - so neither shape exists.
    public static TState Graph<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root, TState seed, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => root is null || !visited.Add(root)
            ? seed
            : TOrderStrategy.Evaluate<
                TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, TAlgebra, TState>(
                root, seed, new TrackedVisitGuard<TNode>(visited));
}
