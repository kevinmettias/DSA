namespace DSAExperimentation.Algorithms.Traversal.BreadthFirst;

// Distinct from IBreadthFirstHooks.Visit, not a convenience wrapper over it: a level
// isn't known to be complete until every node at that depth has been discovered, so
// this needs to buffer a whole depth before firing rather than firing per node.
//
// OnLevel is an instance member on a struct type parameter, and required - see
// IDepthFirstHooks for why, and for how a walk hands its hook value back.
/// <summary>The hook a level-grouped breadth-first walk calls once per level with all of its nodes.</summary>
public interface ILevelGroupedHooks<TNode>
{
    /// <summary>Observes one complete level, once every node at that depth has been found.</summary>
    /// <param name="level">Every node at this depth, in breadth-first order.</param>
    /// <param name="depth">The level's depth below the walk's root, 0 for the root's own level.</param>
    void OnLevel(IReadOnlyList<TNode> level, int depth);
}
