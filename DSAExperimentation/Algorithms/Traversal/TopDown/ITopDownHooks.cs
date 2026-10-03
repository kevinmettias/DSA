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
internal interface ITopDownHooks<TNode, TState>
    where TNode : class
{
    static abstract void Visit(TNode node, TState state, int depth, NodePosition position);

    static abstract TState Descend(TNode parent, TState parentState, TNode child);
}
