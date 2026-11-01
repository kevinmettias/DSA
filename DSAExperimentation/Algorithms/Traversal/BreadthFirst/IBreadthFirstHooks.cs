namespace DSAExperimentation.Algorithms.Traversal.BreadthFirst;

// One event per node - not "not yet implemented", but structural: breadth-first has
// no equivalent to DFS's Exit. Exit means "every descendant of this node is done",
// a moment that only exists because of the call stack; in breadth-first a node's
// children are visited on a later level, interleaved with unrelated cousins, so
// there's no point where "this node's subtree just closed" is meaningfully true.
//
// Visit is an instance member on a struct type parameter, and required - see
// IDepthFirstHooks for why, and for how a walk hands its hook value back.
/// <summary>The hook a breadth-first walk calls once for every node it visits.</summary>
public interface IBreadthFirstHooks<TNode>
{
    /// <summary>Observes one node as the walk reaches it, in breadth-first order.</summary>
    /// <param name="node">The node being visited.</param>
    /// <param name="depth">The node's depth below the walk's root, 0 for the root.</param>
    void Visit(TNode node, int depth);
}
