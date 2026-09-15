namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// LC 1514's undirected input (n, edges, succProb) materialized once, in the two views
// the problem's two strategies need:
//
//   Adjacency - the raw probability per edge, which is what an exhaustive walk over
//               every source-to-target path multiplies along the way;
//   Nodes     - the same edges carrying -log(probability) instead, the non-negative
//               reweighting ShortestPath.Dijkstra runs over.
//
// Both views are built from one pass so a benchmark can charge the whole construction
// to [GlobalSetup] and hand each strategy its prepared input (§17.4). This is not an
// IEnumerable, so the prepared-input overloads can never be ambiguous with the
// LeetCode-shaped ones.
internal sealed class ProbabilityGraph
{
    public ProbabilityNode[] Nodes { get; }

    public List<(double Probability, int To)>[] Adjacency { get; }

    public int NodeCount => Nodes.Length;

    private ProbabilityGraph(ProbabilityNode[] nodes, List<(double Probability, int To)>[] adjacency)
    {
        Nodes = nodes;
        Adjacency = adjacency;
    }

    public static ProbabilityGraph Build(int nodeCount, int[][] edges, double[] successProbabilities)
    {
        var nodes = new ProbabilityNode[nodeCount];
        var adjacency = new List<(double Probability, int To)>[nodeCount];

        for (var id = 0; id < nodeCount; id++)
        {
            nodes[id] = new ProbabilityNode(id);
            adjacency[id] = [];
        }

        var graph = new ProbabilityGraph(nodes, adjacency);

        for (var i = 0; i < edges.Length; i++)
        {
            graph.Connect(edges[i][0], edges[i][1], successProbabilities[i]);
        }

        return graph;
    }

    // LC 1514's edges are undirected, so each one is recorded from both ends.
    private void Connect(int a, int b, double probability)
    {
        var cost = -Math.Log(probability);

        Adjacency[a].Add((probability, b));
        Adjacency[b].Add((probability, a));
        Nodes[a].Edges.Add((cost, Nodes[b]));
        Nodes[b].Edges.Add((cost, Nodes[a]));
    }
}
