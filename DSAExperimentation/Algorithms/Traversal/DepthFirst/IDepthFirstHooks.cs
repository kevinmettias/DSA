namespace DSAExperimentation.Algorithms.Traversal.DepthFirst;

// Enter fires before a node's children are walked, Exit after. Both are optional
// and default to no-ops, so pre-order is "override Enter only", post-order is
// "override Exit only", and a telemetry-style walk (open a span, recurse, close it)
// overrides both - they're not three different traversals, just three ways of using
// this one contract.
internal interface IDepthFirstHooks<TNode>
{
    static virtual void Enter(TNode node, int depth)
    {
    }

    static virtual void Exit(TNode node, int depth)
    {
    }
}
