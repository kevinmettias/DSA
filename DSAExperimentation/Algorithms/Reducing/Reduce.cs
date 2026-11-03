using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Reducing;

// One entry point per topology tier, and the gate DepthFirstTraversal/
// BreadthFirstTraversal ride too, since each is a reduce whose state is its hook:
// which guard gets wired in is chosen by the compiler, not exposed as a
// caller-visible parameter, so an unguarded reduce over a topology that doesn't
// prove unique ancestry is unrepresentable. Tree
// only compiles against ITreeTopology and always uses UnguardedVisit; Graph is the
// only path into a cyclic/shared topology, and it always pays for
// TrackedVisitGuard.
//
// Traversal order is the orthogonal axis, injected as a TOrderStrategy (see
// IReduceOrderStrategy) exactly the way TreeFold injects TStrategy - no default
// here (unlike TreeFold.Fold's recursive default), since order can change a
// reduce's result and callers must choose deliberately.
/// <summary>Threads one state through every node of a tree or graph walk, in a caller-chosen order.</summary>
public static class Reduce
{
    /// <summary>Reduces a tree in the given child order, from the algebra's seed.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <returns>The state after every node has been visited; <c>TAlgebra.Seed</c> when root is null.</returns>
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

    // In the topology's own child order.
    /// <summary>Reduces a tree in the topology's own child order, from the algebra's seed.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <returns>The state after every node has been visited; <c>TAlgebra.Seed</c> when root is null.</returns>
    public static TState Tree<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => Tree<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TOrderStrategy, TAlgebra, TState>(
            root);

    // An explicit starting state, for a reduce whose seed belongs to the call rather than to the
    // algebra alone - a traversal threading its own hook value through the walk is one. An empty
    // walk answers with that seed, not TAlgebra.Seed: the caller's starting state is what nothing
    // was folded into.
    /// <summary>Reduces a tree in the given child order, from a caller-supplied state.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <returns>The state after every node has been visited; <paramref name="seed"/> when root is null.</returns>
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

    // In the topology's own child order.
    /// <summary>Reduces a tree in the topology's own child order, from a caller-supplied state.</summary>
    /// <param name="root">The tree's root, or null for an empty tree.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <returns>The state after every node has been visited; <paramref name="seed"/> when root is null.</returns>
    public static TState Tree<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root, TState seed)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => Tree<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TOrderStrategy, TAlgebra, TState>(
            root, seed);

    /// <summary>Reduces each node a root reaches, in the given child order, from the algebra's seed.</summary>
    /// <param name="root">The node to start from, or null for an empty walk.</param>
    /// <returns>
    /// The state after each node the root reaches has been visited once; <c>TAlgebra.Seed</c> when root is
    /// null.
    /// </returns>
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

    // In the topology's own child order.
    /// <summary>
    /// Reduces each node a root reaches, in the topology's own child order, from the algebra's seed.
    /// </summary>
    /// <param name="root">The node to start from, or null for an empty walk.</param>
    /// <returns>
    /// The state after each node the root reaches has been visited once; <c>TAlgebra.Seed</c> when root is
    /// null.
    /// </returns>
    public static TState Graph<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => Graph<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TOrderStrategy, TAlgebra, TState>(
            root);

    // The multi-root overload: visited is supplied by the caller instead of
    // constructed fresh, so it can be carried across several separate top-level
    // calls - what a whole-graph algorithm (counting connected components) needs,
    // and the single-root overload above structurally
    // cannot express, since that one owns its guard's backing set for the
    // duration of exactly one call. A root already present in visited is treated
    // as nothing to do (the seed, no walk) - the caller's job is deciding
    // whether that's expected (see ConnectedComponents, which checks first and
    // skips instead of calling this at all) or not.
    //
    // Several roots in one reduce, each entered at depth 0 unless an earlier one reached it first.
    // Breadth-first walks them as one frontier, which is the multi-source search: a node's depth is
    // its edge count from the nearest root. Depth-first walks them in turn. Graph tier only, since
    // two roots can reach the same node in any topology.
    /// <summary>Reduces every node the roots reach, in the given child order, walking from all of them at once.</summary>
    /// <param name="roots">The nodes to start from, each at depth 0; a repeated or already-reached root starts nothing.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <returns>The state after each reached node has been visited once; <paramref name="seed"/> when there are no roots.</returns>
    public static TState GraphFromAll<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy, TAlgebra, TState>(
        IEnumerable<TNode> roots, TState seed)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => TOrderStrategy.EvaluateFrom<
            TTopology, TChildren, TOrder, TOrderedChildren, TrackedVisitGuard<TNode>, TAlgebra, TState>(
            roots, seed, new TrackedVisitGuard<TNode>([]));

    // In the topology's own child order.
    /// <summary>Reduces every node the roots reach, in the topology's own child order, walking from all of them at once.</summary>
    /// <param name="roots">The nodes to start from, each at depth 0; a repeated or already-reached root starts nothing.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <returns>The state after each reached node has been visited once; <paramref name="seed"/> when there are no roots.</returns>
    public static TState GraphFromAll<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        IEnumerable<TNode> roots, TState seed)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => GraphFromAll<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TOrderStrategy, TAlgebra, TState>(roots, seed);

    // visited always comes last and never without a seed. A (root, visited) overload
    // beside a (root, seed) one would let C#'s tie-break bind a HashSet<TNode> seed as
    // the visited set - silently, not as an ambiguity error - so neither shape exists.
    /// <summary>
    /// Reduces each node a root reaches, in the given child order, sharing a visited set across calls.
    /// </summary>
    /// <param name="root">The node to start from, or null for an empty walk.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <param name="visited">
    /// Nodes walked by earlier calls, which this walk will not enter again; it adds each node it visits.
    /// </param>
    /// <returns>
    /// The state after each newly reached node has been visited once; <paramref name="seed"/> when root is
    /// null or already in <paramref name="visited"/>.
    /// </returns>
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

    // In the topology's own child order.
    /// <summary>
    /// Reduces each node a root reaches, in the topology's own child order, sharing a visited set across calls.
    /// </summary>
    /// <param name="root">The node to start from, or null for an empty walk.</param>
    /// <param name="seed">The state the walk starts from.</param>
    /// <param name="visited">
    /// Nodes walked by earlier calls, which this walk will not enter again; it adds each node it visits.
    /// </param>
    /// <returns>
    /// The state after each newly reached node has been visited once; <paramref name="seed"/> when root is
    /// null or already in <paramref name="visited"/>.
    /// </returns>
    public static TState Graph<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        TNode? root, TState seed, HashSet<TNode> visited)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
        => Graph<
            TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren,
            TOrderStrategy, TAlgebra, TState>(
            root, seed, visited);
}
