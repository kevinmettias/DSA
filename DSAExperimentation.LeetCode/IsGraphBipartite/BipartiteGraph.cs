namespace DSAExperimentation.LeetCode.IsGraphBipartite;

// LC 785's int[][] adjacency, materialized as nodes. LeetCode's own input is
// already symmetric (j appears in graph[i] iff i appears in graph[j]), which is
// exactly the precondition Algorithms.Bipartiteness.BipartiteCheck documents,
// so wiring each listed neighbour in one direction reproduces the undirected
// graph faithfully.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared graph to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class BipartiteGraph
{
    private BipartiteGraph(IReadOnlyList<BipartiteNode> nodes) => Nodes = nodes;

    // Every vertex, including isolated ones: BipartiteCheck is a multi-root walk
    // and only visits the components its roots reach.
    public IReadOnlyList<BipartiteNode> Nodes { get; }

    public static BipartiteGraph Build(int[][] adjacency)
    {
        var nodes = new BipartiteNode[adjacency.Length];

        for (var id = 0; id < adjacency.Length; id++)
        {
            nodes[id] = new BipartiteNode(id);
        }

        for (var id = 0; id < adjacency.Length; id++)
        {
            foreach (var neighbor in adjacency[id])
            {
                nodes[id].Neighbors.Add(nodes[neighbor]);
            }
        }

        return new BipartiteGraph(nodes);
    }
}
