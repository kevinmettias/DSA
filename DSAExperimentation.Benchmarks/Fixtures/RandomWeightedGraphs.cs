namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a connected, non-negative-weight random graph shaped like the Network
// Delay Time (LC 743) / Cheapest Flights (LC 787) family: every node i > 0 gets a
// "back edge" to some earlier node j < i (guaranteeing reachability from node 0),
// plus extra random forward/back edges for density - so Dijkstra, Bellman-Ford,
// and Floyd-Warshall are all comparing real, non-trivial shortest-path work on the
// exact same graph instance.
internal static class RandomWeightedGraphs
{
    public static (List<WeightedGraphNode> Vertices, WeightedGraphNode Source) Build(
        int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var nodes = Enumerable.Range(0, nodeCount).Select(id => new WeightedGraphNode(id)).ToList();

        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            AddEdge(nodes[j], nodes[i], random);
        }

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

        return (nodes, nodes[0]);
    }

    private static void AddEdge(WeightedGraphNode from, WeightedGraphNode to, Random random)
        => from.Edges.Add((random.Next(1, 50), to));
}
