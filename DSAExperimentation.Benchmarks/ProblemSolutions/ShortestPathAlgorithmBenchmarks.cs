using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The "which shortest-path algorithm" choice a problem like Network Delay Time
// (LC 743) or Cheapest Flights Within K Stops (LC 787) actually forces: all three
// answer the same single-source-distances question on the same non-negative-weight
// graph, so this is a genuine algorithm swap, not an apples-to-oranges comparison.
// FloydWarshall computes every pair at once (a strictly harder question than the
// other two answer) - it is expected to lose here, and the point of including it is
// exactly that: showing when the all-pairs algorithm is the wrong tool for a
// single-source query, not just how fast it is in isolation.
[MemoryDiagnoser]
public class ShortestPathAlgorithmBenchmarks
{
    [Params(50, 300)]
    public int NodeCount;

    private List<WeightedGraphNode> _vertices = null!;
    private WeightedGraphNode _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, source) = RandomWeightedGraphs.Build(NodeCount, extraEdgesPerNode: 3, seed: 42);
        _vertices = vertices;
        _source = source;
    }

    [Benchmark(Baseline = true)]
    public int Dijkstra()
        => ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(_source).Count;

    [Benchmark]
    public int BellmanFord()
    {
        DSAExperimentation.Algorithms.ShortestPaths.BellmanFord.TryComputeDistances<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(
            _vertices, _source, out var distances);

        return distances.Count;
    }

    [Benchmark]
    public int FloydWarshall()
    {
        AllPairsShortestPaths.TryComputeDistances<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(
            _vertices, out var distances);

        return distances.Count;
    }
}
