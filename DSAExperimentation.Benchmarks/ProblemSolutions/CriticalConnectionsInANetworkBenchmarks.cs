using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Critical Connections in a Network (LC 1192): the textbook "remove each edge,
// rerun a full BFS to check connectivity" brute force, O(E*(V+E)) - against this
// repo's own low-link bridge-finding DFS (BridgesAndArticulationPoints.Find),
// O(V+E) total. The network is a chain of triangles linked end-to-end (a repeating
// two-triangle-joined-by-one-edge shape, the same construction
// BridgesAndArticulationPointsTests uses) so most edges sit on a real cycle
// (never a bridge) while the links between triangles are real bridges - both
// strategies see a genuine mix instead of an all-bridge or no-bridge shortcut.
[MemoryDiagnoser]
public class CriticalConnectionsInANetworkBenchmarks
{
    private const int TriangleSize = 3;

    [Params(150, 3_000)]
    public int NodeCount;

    private List<(int A, int B)> _connections = null!;

    [GlobalSetup]
    public void Setup()
    {
        var connections = new List<(int A, int B)>();
        var groupCount = NodeCount / TriangleSize;

        for (var g = 0; g < groupCount; g++)
        {
            var first = g * TriangleSize;
            connections.Add((first, first + 1));
            connections.Add((first + 1, first + 2));
            connections.Add((first + 2, first));

            if (g > 0)
            {
                connections.Add((first - 1, first));
            }
        }

        _connections = connections;
    }

    [Benchmark(Baseline = true)]
    public int NaiveEdgeRemovalScan()
    {
        var bridgeCount = 0;

        for (var i = 0; i < _connections.Count; i++)
        {
            if (!IsConnectedWithoutEdge(i))
            {
                bridgeCount++;
            }
        }

        return bridgeCount;
    }

    [Benchmark]
    public int LowLinkBridgeSearch()
    {
        var servers = Enumerable.Range(0, NodeCount).Select(id => new ServerNode(id)).ToList();

        foreach (var (a, b) in _connections)
        {
            servers[a].Neighbors.Add(servers[b]);
            servers[b].Neighbors.Add(servers[a]);
        }

        var (bridges, _) = BridgesAndArticulationPoints.Find<
            ServerNode, ServerTopology, ListChildren<ServerNode>,
            NaturalChildOrder<ServerNode, ListChildren<ServerNode>>, ListChildren<ServerNode>>(
            servers);

        return bridges.Count;
    }

    private bool IsConnectedWithoutEdge(int skipIndex)
    {
        var adjacency = BuildAdjacencyWithoutEdge(skipIndex);
        var visited = new bool[NodeCount];
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;
        var visitedCount = 1;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in adjacency[current])
            {
                if (visited[neighbor])
                {
                    continue;
                }

                visited[neighbor] = true;
                visitedCount++;
                queue.Enqueue(neighbor);
            }
        }

        return visitedCount == NodeCount;
    }

    private List<int>[] BuildAdjacencyWithoutEdge(int skipIndex)
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 0; i < _connections.Count; i++)
        {
            if (i == skipIndex)
            {
                continue;
            }

            var (a, b) = _connections[i];
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency;
    }

    // See CriticalConnectionsInANetworkTests.Fixtures for the full explanation -
    // repeated here rather than shared because TwoSumBenchmarks/
    // CourseScheduleIIBenchmarks establish this project keeps its own copy of the
    // solution rather than depending on the Tests project.
    private sealed class ServerNode(int id)
    {
        public int Id { get; } = id;

        public List<ServerNode> Neighbors { get; } = [];
    }

    private readonly struct ServerTopology : IGraphTopology<ServerNode, ListChildren<ServerNode>>
    {
        public static ListChildren<ServerNode> GetChildren(ServerNode node) => new(node.Neighbors);
    }
}
