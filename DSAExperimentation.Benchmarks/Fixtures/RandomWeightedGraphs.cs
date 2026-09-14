namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a connected, non-negative-weight random graph shaped like the Network
// Delay Time (LC 743) / Cheapest Flights (LC 787) family: every node i > 0 gets a
// "back edge" to some earlier node j < i (guaranteeing reachability from node 0),
// plus extra random forward/back edges for density - so Dijkstra, Bellman-Ford,
// and Floyd-Warshall are all comparing real, non-trivial shortest-path work on the
// exact same graph instance.
internal static class RandomWeightedGraphs
{
    // Exclusive upper bound passed to Random.Next(1, _): edge weights land in [1, 49].
    private const int EdgeWeightUpperBound = 50;

    public static (List<WeightedGraphNode> Vertices, WeightedGraphNode Source) Build(
        int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var nodes = CreateNodes(nodeCount);

        AddBackEdges(nodes, nodeCount, random);
        AddExtraEdges(nodes, nodeCount, extraEdgesPerNode, random);

        return (nodes, nodes[0]);
    }

    // The same graph, flattened back into LeetCode's own [from, to, weight] edge
    // list, for problems whose input shape is the edge list rather than a built
    // node graph (Design Graph With Shortest Path Calculator, LC 2642). Same seed
    // and same density, so the two forms are the identical workload.
    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var (vertices, _) = Build(nodeCount, extraEdgesPerNode, seed);
        var edges = new List<int[]>();

        foreach (var node in vertices)
        {
            foreach (var (weight, target) in node.Edges)
            {
                edges.Add([node.Id, target.Id, weight]);
            }
        }

        return [.. edges];
    }

    private static List<WeightedGraphNode> CreateNodes(int nodeCount)
        => Enumerable.Range(0, nodeCount).Select(id => new WeightedGraphNode(id)).ToList();

    private static void AddBackEdges(List<WeightedGraphNode> nodes, int nodeCount, Random random)
    {
        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            AddEdge(nodes[j], nodes[i], random);
        }
    }

    private static void AddExtraEdges(List<WeightedGraphNode> nodes, int nodeCount, int extraEdgesPerNode, Random random)
    {
        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    AddEdge(nodes[i], nodes[target], random);
                }
            }
        }
    }

    private static void AddEdge(WeightedGraphNode from, WeightedGraphNode to, Random random)
        => from.Edges.Add((random.Next(1, EdgeWeightUpperBound), to));
}
