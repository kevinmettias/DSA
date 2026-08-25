namespace DSAExperimentation.Algorithms.Graph.Engines.Traversal.BreadthFirst;

// Distinct from IBreadthFirstHooks.Visit, not a convenience wrapper over it: a level
// isn't known to be complete until every node at that depth has been discovered, so
// this needs to buffer a whole depth before firing rather than firing per node.
internal interface ILevelGroupedHooks<TNode>
{
    static abstract void OnLevel(IReadOnlyList<TNode> level, int depth);
}
