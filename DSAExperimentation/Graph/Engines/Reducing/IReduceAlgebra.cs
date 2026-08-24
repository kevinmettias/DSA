namespace DSAExperimentation.Graph.Engines.Reducing;

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
internal interface IReduceAlgebra<TNode, TState>
{
    static abstract TState Seed { get; }

    static virtual TState Enter(TState state, TNode node, int depth)
        => state;

    static virtual TState Exit(TState state, TNode node, int depth)
        => state;
}
