namespace DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

// LC 2493's (n, edges) input materialized as GroupNode objects. The edges are
// undirected, so every listed pair is wired in both directions - which is
// exactly the symmetric-adjacency precondition
// Algorithms.Bipartiteness.BipartiteCheck documents.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared graph to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class GroupGraph
{
    // Every node, including any that no edge touches: the answer sums each
    // connected component's own best grouping, and an isolated node is a
    // component contributing one group.
    public IReadOnlyList<GroupNode> Nodes { get; }

    private GroupGraph(IReadOnlyList<GroupNode> nodes) => Nodes = nodes;

    public static GroupGraph Build(int nodeCount, int[][] edges)
    {
        // Slot i holds node i, so LeetCode's 1..n numbering indexes directly;
        // slot 0 is an unused placeholder that carries no edges and is dropped
        // before the graph is returned.
        var byId = Enumerable.Range(0, nodeCount + 1).Select(id => new GroupNode(id)).ToList();

        foreach (var edge in edges)
        {
            byId[edge[0]].Neighbors.Add(byId[edge[1]]);
            byId[edge[1]].Neighbors.Add(byId[edge[0]]);
        }

        return new GroupGraph(byId.Skip(1).ToList());
    }
}
