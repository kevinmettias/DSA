namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for Shortest Path Visiting All Nodes (LC 847): a
// random connected undirected graph (every node i > 0 gets a "back edge" to some
// earlier node j < i, guaranteeing connectivity, plus a few extra random edges
// for density - the same spanning-tree-plus-extras shape RandomWeightedGraphs.Build
// already uses for the Dijkstra/BellmanFord/FloydWarshall benchmarks).
//
// Only the size and the seed live here. Turning an adjacency list into the
// (node, visited-mask) state space is the problem's own structure, so it is
// LeetCode.ShortestPathVisitingAllNodes.VisitStateGraph's job (ARCHITECTURE.md
// 17.7).
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

    private static void AddEdge(List<int>[] adjacency, int firstNode, int secondNode)
    {
        adjacency[firstNode].Add(secondNode);
        adjacency[secondNode].Add(firstNode);
    }
}
