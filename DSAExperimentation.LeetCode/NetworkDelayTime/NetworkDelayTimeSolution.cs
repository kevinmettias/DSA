using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NetworkDelayTime;

// LeetCode 743. Network Delay Time: minutes for a signal sent from the source
// node to reach every one of the nodeCount nodes, or -1 if some node is
// unreachable. That is a direct read of a single-source shortest-path
// algorithm's own distance map - the longest distance, once every node has one,
// or "unreachable" the moment one doesn't. All three strategies below answer
// that same question on the same directed, non-negative-weight graph;
// ShortestPathAlgorithmBenchmarks (see its own doc comment) exists specifically
// to compare them, with FloydWarshall expected to lose because it answers the
// strictly harder all-pairs question.
//
// Each strategy's prepared-input overload stays generic over TNode/TTopology rather
// than fixing NetworkNode/NetworkTopology: the reduction ("every node reached? take
// the max; otherwise -1") is exactly as generic as ShortestPath.Dijkstra itself, and
// staying generic lets ShortestPathAlgorithmBenchmarks hand these methods the same
// random WeightedGraphNode graph several other LeetCode problems' benchmarks already
// share, instead of forcing yet another copy of that representation into this tier.
internal static class NetworkDelayTimeSolution
{
    // The textbook single-source choice for non-negative weights, and the one the
    // other two strategies are compared against.
    public static int MinutesToReachAllByDijkstra(int[][] times, int nodeCount, int sourceNodeId)
    {
        var (vertices, source) = BuildGraph(times, nodeCount, sourceNodeId);

        return MinutesToReachAllByDijkstra<NetworkNode, NetworkTopology>(vertices, source);
    }

    public static int MinutesToReachAllByDijkstra<TNode, TTopology>(List<TNode> vertices, TNode source)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, ListEdges<TNode, int>, int>
    {
        var distances = ShortestPath.Dijkstra<TNode, TTopology, ListEdges<TNode, int>, int>(source);

        return DistancesToAnswer(vertices, distances);
    }

    // Correct here precisely because LC 743's weights are guaranteed non-negative, so
    // the extra round Bellman-Ford spends detecting a negative cycle never fires.
    public static int MinutesToReachAllByBellmanFord(int[][] times, int nodeCount, int sourceNodeId)
    {
        var (vertices, source) = BuildGraph(times, nodeCount, sourceNodeId);

        return MinutesToReachAllByBellmanFord<NetworkNode, NetworkTopology>(vertices, source);
    }

    public static int MinutesToReachAllByBellmanFord<TNode, TTopology>(List<TNode> vertices, TNode source)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, ListEdges<TNode, int>, int>
    {
        DSAExperimentation.Algorithms.ShortestPaths.BellmanFord.TryComputeDistances<
            TNode, TTopology, ListEdges<TNode, int>, int>(vertices, source, out var distances);

        return DistancesToAnswer(vertices, distances);
    }

    // Answers the strictly harder all-pairs question and reads off just the row for
    // `source` - included as a strategy because it is a genuine way to compute the
    // same answer, not because it is a good one; see ShortestPathAlgorithmBenchmarks
    // for the point of measuring it anyway.
    public static int MinutesToReachAllByFloydWarshall(int[][] times, int nodeCount, int sourceNodeId)
    {
        var (vertices, source) = BuildGraph(times, nodeCount, sourceNodeId);

        return MinutesToReachAllByFloydWarshall<NetworkNode, NetworkTopology>(vertices, source);
    }

    public static int MinutesToReachAllByFloydWarshall<TNode, TTopology>(List<TNode> vertices, TNode source)
        where TNode : class
        where TTopology : struct, IEdgeTopology<TNode, ListEdges<TNode, int>, int>
    {
        AllPairsShortestPaths.TryComputeDistances<
            TNode, TTopology, ListEdges<TNode, int>, int>(vertices, out var pairDistances);

        var max = 0;

        foreach (var vertex in vertices)
        {
            if (!pairDistances.TryGetValue((source, vertex), out var distance))
            {
                return LeetCodeAnswer.None;
            }

            max = Math.Max(max, distance);
        }

        return max;
    }

    private static int DistancesToAnswer<TNode>(List<TNode> vertices, Dictionary<TNode, int> distances)
        where TNode : class
        => distances.Count == vertices.Count ? distances.Values.Max() : LeetCodeAnswer.None;

    private static (List<NetworkNode> Vertices, NetworkNode Source) BuildGraph(
        int[][] times, int nodeCount, int sourceNodeId)
    {
        var nodes = new Dictionary<int, NetworkNode>();

        for (var id = 1; id <= nodeCount; id++)
        {
            nodes[id] = new NetworkNode(id);
        }

        foreach (var edge in times)
        {
            nodes[edge[0]].Edges.Add((edge[2], nodes[edge[1]]));
        }

        return (nodes.Values.ToList(), nodes[sourceNodeId]);
    }
}
