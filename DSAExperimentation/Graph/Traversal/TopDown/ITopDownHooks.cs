namespace DSAExperimentation.Graph;

// The inherited-attribute counterpart to IReduceAlgebra's synthesized one:
// IReduceAlgebra threads a single accumulator sequentially across the WHOLE walk
// (proven by ReduceTests' cross-subtree test - a later sibling sees an earlier
// sibling's finished state). This threads a value DOWN a single root-to-node path -
// Descend computes each child's state independently from its parent's, so a sibling
// never sees another sibling's state at all. That's what root-to-leaf problems
// (a running sum, a decreasing budget, the path so far) actually need and
// IReduceAlgebra structurally cannot express.
//
// No Seed/Empty-style static starting value the way IFoldAlgebra/IReduceAlgebra
// have: the state a root starts with is exactly the kind of thing this primitive
// exists to carry (see AllRootToLeafPaths, which seeds it with a fresh, per-call
// output list) - baking it into a static-abstract property would reintroduce the
// same runtime-parameterization wall LowestCommonAncestor hit with IFoldAlgebra.
// TopDownTraversal.Walk/WalkGraph take it as an ordinary parameter instead.
public interface ITopDownHooks<TNode, TState>
    where TNode : class
{
    static abstract void Visit(TNode node, TState state, int depth, NodePosition position);

    static abstract TState Descend(TNode parent, TState parentState, TNode child);
}
