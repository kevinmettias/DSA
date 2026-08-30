namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathVisitingAllNodes.Fixtures;

// One node per (current node, bitmask of nodes visited so far) state; Neighbors
// holds every state reachable by taking one edge of the input graph from here,
// filled in once while the graph is built - mirrors OpenTheLockTests' LockNode.
internal sealed class VisitStateNode(int node, int mask)
{
    public int Node { get; } = node;

    public int Mask { get; } = mask;

    public List<VisitStateNode> Neighbors { get; } = [];
}
