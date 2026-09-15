using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NetworkDelayTime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The "which shortest-path algorithm" choice a problem like Network Delay Time
// (LC 743) or Cheapest Flights Within K Stops (LC 787) actually forces: all three
// arms are NetworkDelayTimeSolution's own strategies, the same methods
// NetworkDelayTimeTests proves correct, called through their generic prepared-input
// overload so graph construction is charged to [GlobalSetup] rather than to the
// search being measured. All three answer the same single-source-distances question
// on the same non-negative-weight graph, so this is a genuine algorithm swap, not an
// apples-to-oranges comparison. FloydWarshall computes every pair at once (a
// strictly harder question than the other two answer) - it is expected to lose here,
// and the point of including it is exactly that: showing when the all-pairs
// algorithm is the wrong tool for a single-source query, not just how fast it is in
// isolation.
[MemoryDiagnoser]
public class ShortestPathAlgorithmBenchmarks
{
    private const int ExtraEdgesPerNode = 3;
    private const int RandomSeed = 42;

    private List<WeightedGraphNode> _vertices = new();

    private WeightedGraphNode _source = null!;
    [Params(50, 300)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, source) = RandomWeightedGraphs.Build(NodeCount, extraEdgesPerNode: ExtraEdgesPerNode, seed: RandomSeed);
        _vertices = vertices;
        _source = source;
    }

    [Benchmark(Baseline = true)]
    public int Dijkstra()
        => NetworkDelayTimeSolution.MinutesToReachAllByDijkstra<WeightedGraphNode, WeightedGraphTopology>(_vertices, _source);

    [Benchmark]
    public int BellmanFord()
        => NetworkDelayTimeSolution.MinutesToReachAllByBellmanFord<WeightedGraphNode, WeightedGraphTopology>(_vertices, _source);

    [Benchmark]
    public int FloydWarshall()
        => NetworkDelayTimeSolution.MinutesToReachAllByFloydWarshall<WeightedGraphNode, WeightedGraphTopology>(_vertices, _source);
}
