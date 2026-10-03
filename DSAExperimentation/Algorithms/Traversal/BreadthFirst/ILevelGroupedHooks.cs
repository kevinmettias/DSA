namespace DSAExperimentation.Algorithms.Traversal.BreadthFirst;

// Distinct from IBreadthFirstHooks.Visit, not a convenience wrapper over it: a level
// isn't known to be complete until every node at that depth has been discovered, so
// this needs to buffer a whole depth before firing rather than firing per node.
//
// OnLevel is an instance member on a struct type parameter, and required - see
// IDepthFirstHooks for why, and for how a walk hands its hook value back.
internal interface ILevelGroupedHooks<TNode>
{
    void OnLevel(IReadOnlyList<TNode> level, int depth);
}
