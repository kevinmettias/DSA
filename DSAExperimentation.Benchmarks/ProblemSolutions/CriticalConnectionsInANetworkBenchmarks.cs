using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CriticalConnectionsInANetworkSolution's, the same
// methods CriticalConnectionsInANetworkTests proves correct - the textbook "remove
// each connection, re-run a full BFS to check connectivity" scan, O(E*(V+E)),
// against this repo's low-link bridge-finding DFS
// (Algorithms.Connectivity.BridgesAndArticulationPoints.Find), O(V+E) total.
//
// The network is a chain of triangles linked end-to-end (the same construction
// BridgesAndArticulationPointsTests uses) so most connections sit on a real cycle
// and are never critical, while the links between triangles are real bridges -
// both strategies see a genuine mix instead of an all-bridge or no-bridge
// shortcut.
//
// Materializing the ServerNode network is input construction, so it is charged to
// [GlobalSetup] and handed to the strategy's prepared-input overload; the baseline
// takes the same connection list in LeetCode's own int[][] shape, which needs no
// preparation.
[MemoryDiagnoser]
public class CriticalConnectionsInANetworkBenchmarks
{
    private const int TriangleSize = 3;

    // Offset of a triangle's third vertex from its first, within a group of
    // TriangleSize nodes.
    private const int ThirdVertexOffset = TriangleSize - 1;

    [Params(150, 3_000)]
    public int NodeCount;

    private int[][] _connections = null!;
    private ServerNetwork _network = null!;

    [GlobalSetup]
    public void Setup()
    {
        var connections = new List<int[]>();
        var groupCount = NodeCount / TriangleSize;

        for (var g = 0; g < groupCount; g++)
        {
            var first = g * TriangleSize;
            connections.Add([first, first + 1]);
            connections.Add([first + 1, first + ThirdVertexOffset]);
            connections.Add([first + ThirdVertexOffset, first]);

            if (g > 0)
            {
                connections.Add([first - 1, first]);
            }
        }

        _connections = [.. connections];
        _network = ServerNetwork.Build(NodeCount, _connections);
    }

    [Benchmark(Baseline = true)]
    public int NaiveEdgeRemovalScan() =>
        CriticalConnectionsInANetworkSolution
            .CriticalConnectionsByEdgeRemovalScan(NodeCount, _connections).Length;

    [Benchmark]
    public int LowLinkBridgeSearch() =>
        CriticalConnectionsInANetworkSolution.CriticalConnectionsByLowLinkSearch(_network).Length;
}
