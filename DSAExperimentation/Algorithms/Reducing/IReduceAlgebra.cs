namespace DSAExperimentation.Algorithms.Reducing;

// Mirrors IDepthFirstHooks.Enter/Exit exactly, but state-threading instead of void:
// a running accumulator passes through Enter and/or Exit rather than a node being
// combined with its already-computed children. That's what makes reduce genuinely
// order-dependent - which IReduceTraversalStrategy you pick can change the answer,
// not just how fast you get it (contrast IFoldAlgebra).
//
// Both hooks default to identity, so overriding only Enter gives you a pre-order
// reduce, only Exit a post-order reduce, and both together lets a single reduce see
// matched open/close events - e.g. tracking concurrently-open nodes, which no
// single-event-per-node shape could express. Breadth-first has no Exit moment (see
// IBreadthFirstHooks), so a breadth-first strategy simply never calls it.
//
// IsDone is how a reduce stops early, and it belongs to the state because the state is
// what knows: a search that has found its target, a count that has reached k. A walk asks
// it after each event and, once it answers true, makes no further Enter or Exit call and
// hands the state back as it stands. It asks only when the algebra's StopsEarly says to:
// every node type here is a reference type, so a walk runs as shared generic code where a
// call to TAlgebra's static members is a runtime lookup, and asking IsDone at every node
// measured 14% on a walk that never stops (OperationsOnTree). StopsEarly is read once per
// walk instead, by the order strategy, which picks the walk's stop check from it (see
// IStopCheck): an algebra that never stops gets the walk with no check in it, and pays at
// most one lookup per walk, none where the strategy inlines into exact types. An algebra
// that can stop overrides both.
/// <summary>
/// The per-node step of a reduce, threading one running state through Enter and Exit at each node visited.
/// </summary>
public interface IReduceAlgebra<TNode, TState>
{
    /// <summary>The state a reduce starts from when its caller supplies none.</summary>
    static abstract TState Seed { get; }

    /// <summary>Folds a node into the state as the walk reaches it, before any of its children.</summary>
    /// <param name="state">The state so far.</param>
    /// <param name="node">The node being entered.</param>
    /// <param name="depth">The node's edge count from the walk's root, along the path the walk reached it by.</param>
    /// <returns>The state with this node folded in; the default returns <paramref name="state"/> unchanged.</returns>
    static virtual TState Enter(TState state, TNode node, int depth)
        => state;

    /// <summary>
    /// Folds a node into the state as a depth-first walk leaves it; breadth-first walks never call it.
    /// </summary>
    /// <param name="state">The state so far, after the node's whole subtree.</param>
    /// <param name="node">The node being left.</param>
    /// <param name="depth">The depth the node was entered at.</param>
    /// <returns>
    /// The state with this node's exit folded in; the default returns <paramref name="state"/> unchanged.
    /// </returns>
    static virtual TState Exit(TState state, TNode node, int depth)
        => state;

    /// <summary>Whether a walk should ask <c>IsDone</c> at all; read once per walk.</summary>
    /// <returns>True for an algebra that can stop early; the default never asks.</returns>
    static virtual bool StopsEarly
        => false;

    /// <summary>Whether the reduce has its answer, so the walk can stop before its next event.</summary>
    /// <param name="state">The state after the event just folded in.</param>
    /// <returns>True to end the walk with <paramref name="state"/>; the default never ends it early.</returns>
    static virtual bool IsDone(TState state)
        => false;
}
