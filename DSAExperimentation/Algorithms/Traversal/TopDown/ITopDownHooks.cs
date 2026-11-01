namespace DSAExperimentation.Algorithms.Traversal.TopDown;

// The inherited-attribute counterpart to IReduceAlgebra's synthesized one:
// IReduceAlgebra threads a single accumulator sequentially across the WHOLE walk
// (proven by ReduceTests' cross-subtree test - a later sibling sees an earlier
// sibling's finished state). This threads a value DOWN a single root-to-node path -
// Descend computes each child's state independently from its parent's, so a sibling
// never sees another sibling's state at all. That's what root-to-leaf problems
// (a running sum, a decreasing budget, the path so far) actually need and
// IReduceAlgebra structurally cannot express.
//
// No Seed-style static starting value the way IReduceAlgebra has: the state a root
// starts with is exactly the kind of thing this primitive exists to carry (see
// AllRootToLeafPaths, which seeds it with a fresh, per-call output list), and a
// static-abstract property, fixed by TYPE, has no channel to a per-call value.
// TopDownTraversal.Walk/WalkGraph take it as an ordinary parameter instead. The hooks
// themselves stay static-abstract, unlike IFoldAlgebra's instance members: every
// runtime value they need arrives through Visit's and Descend's own parameters.
/// <summary>
/// Static callbacks for a top-down walk, which derives each child's state from its parent's alone, never from
/// a sibling's.
/// </summary>
public interface ITopDownHooks<TNode, TState>
    where TNode : class
{
    /// <summary>Called once per node the walk reaches, before the walk enters any of its children.</summary>
    /// <param name="node">The node being visited.</param>
    /// <param name="state">
    /// The initial state at the root, else what <see cref="Descend"/> returned for this node.
    /// </param>
    /// <param name="depth">How many edges lie between the root and the node on the walk's path, 0 at the root.</param>
    /// <param name="position"><c>Leaf</c> when the node has no children, otherwise <c>Interior</c>.</param>
    static abstract void Visit(TNode node, TState state, int depth, NodePosition position);

    /// <summary>Computes the state for a child the walk is entering, from its parent's state.</summary>
    /// <param name="parent">The node whose child the walk is entering, already visited.</param>
    /// <param name="parentState">The state <paramref name="parent"/> was visited with.</param>
    /// <param name="child">The child the walk is entering.</param>
    /// <returns>The state <paramref name="child"/> is visited with and hands on to its own children.</returns>
    static abstract TState Descend(TNode parent, TState parentState, TNode child);
}
