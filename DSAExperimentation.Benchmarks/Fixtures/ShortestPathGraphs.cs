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
        var adjacency = CreateEmptyAdjacency(nodeCount);

        AddSpanningTreeEdges(adjacency, nodeCount, random);
        AddExtraRandomEdges(adjacency, nodeCount, random);

        return ToJaggedArray(adjacency);
    }

    private static List<int>[] CreateEmptyAdjacency(int nodeCount)
    {
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        return adjacency;
    }

    private static void AddSpanningTreeEdges(List<int>[] adjacency, int nodeCount, Random random)
    {
        for (var i = 1; i < nodeCount; i++)
        {
            AddEdge(adjacency, i, random.Next(i));
        }
    }

    private static void AddExtraRandomEdges(List<int>[] adjacency, int nodeCount, Random random)
    {
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
    }

    private static int[][] ToJaggedArray(List<int>[] adjacency) =>
        adjacency.Select(neighbors => neighbors.ToArray()).ToArray();

    public static (Dictionary<(int Node, int Mask), VisitStateNode> NodesByState, VisitStateNode[] StartNodes)
        BuildStateGraph(int[][] graph)
    {
        var nodesByState = CreateStateNodes(graph);

        WireNeighbors(graph, nodesByState);

        var startNodes = CreateStartNodes(graph, nodesByState);

        return (nodesByState, startNodes);
    }

    private static Dictionary<(int Node, int Mask), VisitStateNode> CreateStateNodes(int[][] graph)
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

        return nodesByState;
    }

    private static void WireNeighbors(int[][] graph, Dictionary<(int Node, int Mask), VisitStateNode> nodesByState)
    {
        foreach (var state in nodesByState.Values)
        {
            foreach (var neighbor in graph[state.Node])
            {
                state.Neighbors.Add(nodesByState[(neighbor, state.Mask | (1 << neighbor))]);
            }
        }
    }

    private static VisitStateNode[] CreateStartNodes(int[][] graph, Dictionary<(int Node, int Mask), VisitStateNode> nodesByState)
    {
        var startNodes = new VisitStateNode[graph.Length];

        for (var start = 0; start < graph.Length; start++)
        {
            startNodes[start] = nodesByState[(start, 1 << start)];
        }

        return startNodes;
    }

    private static void AddEdge(List<int>[] adjacency, int a, int b)
    {
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }
}
