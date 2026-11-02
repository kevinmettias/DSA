namespace DSAExperimentation.Algorithms.Reducing;

// How many nodes a walk reaches: Enter adds one, whatever the node and its depth. With the
// multi-root walk ConnectedComponents.ReduceEach rides, that is each component's size - an
// island's area, a region's cell count - which grid solutions were reading off the length of a
// list a lambda traversal had built for them.
/// <summary>Counts the nodes a reduce enters.</summary>
public readonly struct NodeCountReduceAlgebra<TNode> : IReduceAlgebra<TNode, int>
{
    /// <summary>No node counted yet.</summary>
    public static int Seed => 0;

    /// <summary>Counts one more node.</summary>
    /// <param name="state">The count so far.</param>
    /// <param name="node">The node entered, which only adds to the count.</param>
    /// <param name="depth">The node's depth, which the count ignores.</param>
    /// <returns>The count with this node included.</returns>
    public static int Enter(int state, TNode node, int depth) => state + 1;
}
