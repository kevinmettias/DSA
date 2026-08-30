namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Shortest Path Visiting All Nodes scenario (LC 847): a random connected
// undirected graph (every node i > 0 gets a "back edge" to some earlier node j < i,
// guaranteeing connectivity, plus a few extra random edges for density - the same
// spanning-tree-plus-extras shape RandomWeightedGraphs.Build already uses for the
// Dijkstra/BellmanFord/FloydWarshall benchmarks), then every (node, visited-mask)
// pair over it becomes a VisitStateNode, the same "materialize every state, then
// wire neighbors" construction LockGraphs/PuzzleGraphs use for their own state
// spaces.
internal static class ShortestPathGraphs
{
    public static int[][] BuildRandomConnectedGraph(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 1; i < nodeCount; i++)
        {
            AddEdge(adjacency, i, random.Next(i));
        }

        var extraEdgesPerNode = 1;

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i && !adjacency[i].Contains(target))
                {
                    AddEdge(adjacency, i, target);
                }
            }
        }

        return adjacency.Select(neighbors => neighbors.ToArray()).ToArray();
    }

    public static (Dictionary<(int Node, int Mask), VisitStateNode> NodesByState, VisitStateNode[] StartNodes)
        BuildStateGraph(int[][] graph)
    {
        var stateCount = 1 << graph.Length;
        var nodesByState = new Dictionary<(int Node, int Mask), VisitStateNode>();

        for (var node = 0; node < graph.Length; node++)
        {
            for (var mask = 0; mask < stateCount; mask++)
            {
                nodesByState[(node, mask)] = new VisitStateNode(node, mask);
            }
        }

        foreach (var state in nodesByState.Values)
        {
            foreach (var neighbor in graph[state.Node])
            {
                state.Neighbors.Add(nodesByState[(neighbor, state.Mask | (1 << neighbor))]);
            }
        }

        var startNodes = new VisitStateNode[graph.Length];

        for (var start = 0; start < graph.Length; start++)
        {
            startNodes[start] = nodesByState[(start, 1 << start)];
        }

        return (nodesByState, startNodes);
    }

    private static void AddEdge(List<int>[] adjacency, int a, int b)
    {
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }
}
