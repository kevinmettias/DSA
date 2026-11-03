using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.LeetCode.Solutions.Conventions;

// LC 743's own directed, non-negative-weight edge list: Edges holds each node's out-edges as
// (Weight, Target).
using NetworkNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<int>;
using NetworkTopology = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyTopology<int>;

namespace DSAExperimentation.LeetCode.Solutions.NetworkDelayTime;

// LeetCode 743. Network Delay Time: minutes for a signal sent from the source
// node to reach every one of the nodeCount nodes, or -1 if some node is
// unreachable. That is a direct read of a single-source shortest-path
// algorithm's own distance map - the longest distance, once every node has one,
// or "unreachable" the moment one doesn't. Every strategy below answers that same
// question on the same directed, non-negative-weight graph: the textbook Dijkstra
// over a BCL PriorityQueue, and this repo's own Dijkstra, Bellman-Ford and
// Floyd-Warshall. ShortestPathAlgorithmBenchmarks (see its own doc comment) exists
// specifically to compare them, with FloydWarshall expected to lose because it
// answers the strictly harder all-pairs question.
//
// Each strategy's prepared-input overload stays generic over TNode/TTopology rather
// than fixing NetworkNode/NetworkTopology: the reduction ("every node reached? take
// the max; otherwise -1") is exactly as generic as ShortestPath.Dijkstra itself, and
// staying generic keeps the strategies honest about what they depend on: an edge
// topology, not this problem's node type.
[LeetCodeQuestion("network-delay-time", LeetCodeAnswerOrder.AsWritten)]
internal static class NetworkDelayTimeSolution
{
    // LeetCode numbers the nodes 1..nodeCount, so slot 0 of a node-indexed array is an
    // unused placeholder every walk starts past.
    private const int FirstNode = 1;

    // No signal has reached this node yet.
    private const int Unreached = int.MaxValue;

    // The textbook answer: Dijkstra as a BCL PriorityQueue relaxation loop over plain
    // out-edge lists, the arm this repo's three algorithms are measured against
    // (ARCHITECTURE 17.5).
    public static int MinutesToReachAllByPriorityQueue(int[][] times, int nodeCount, int sourceNodeId)
    {
        var outEdges = OutEdgeListsOf(times, nodeCount);
        var minutes = MinutesFrom(outEdges, sourceNodeId);

        return LatestArrivalOrNone(minutes);
    }

    // times[] as one out-edge list per node, each time [from, to, minutes] read in order.
    private static List<(int Target, int Minutes)>[] OutEdgeListsOf(int[][] times, int nodeCount)
    {
        var outEdges = new List<(int Target, int Minutes)>[nodeCount + FirstNode];

        for (var node = 0; node < outEdges.Length; node++)
        {
            outEdges[node] = [];
        }

        foreach (var time in times)
        {
            outEdges[time[0]].Add((time[1], time[2]));
        }

        return outEdges;
    }

    // Each node's shortest signal time from source: a node is final the first time it is
    // popped, and any later, staler entry for it is skipped.
    private static int[] MinutesFrom(List<(int Target, int Minutes)>[] outEdges, int source)
    {
        var minutes = new int[outEdges.Length];
        Array.Fill(minutes, Unreached);
        minutes[source] = 0;

        var frontier = new PriorityQueue<int, int>();
        frontier.Enqueue(source, 0);

        while (frontier.TryDequeue(out var node, out var arrival))
        {
            if (arrival == minutes[node])
            {
                RelaxOutEdges(outEdges[node], arrival, minutes, frontier);
            }
        }

        return minutes;
    }

    private static void RelaxOutEdges(
        List<(int Target, int Minutes)> edges, int arrival, int[] minutes, PriorityQueue<int, int> frontier)
    {
        foreach (var (target, travel) in edges)
        {
            var candidate = arrival + travel;

            if (candidate < minutes[target])
            {
                minutes[target] = candidate;
                frontier.Enqueue(target, candidate);
            }
        }
    }

    // The latest arrival once every node has a time, or "unreachable" the moment one
    // doesn't.
    private static int LatestArrivalOrNone(int[] minutes)
    {
        var latest = 0;

        for (var node = FirstNode; node < minutes.Length; node++)
        {
            if (minutes[node] == Unreached)
            {
                return LeetCodeAnswer.None;
            }

            latest = Math.Max(latest, minutes[node]);
        }

        return latest;
    }

    // This repo's own single-source choice for non-negative weights, and the one
    // Bellman-Ford and Floyd-Warshall are compared against.
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
