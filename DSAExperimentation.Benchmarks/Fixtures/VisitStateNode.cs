namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' ShortestPathVisitingAllNodes VisitStateNode
// fixture: one node per (current node, bitmask of nodes visited so far) state,
// Neighbors holding every state reachable by taking one edge of the input graph.
internal sealed class VisitStateNode(int node, int mask)
{
    public int Node { get; } = node;

    public int Mask { get; } = mask;

    public List<VisitStateNode> Neighbors { get; } = [];
}
