using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostPathWithEdgeReversalsSolution's, the same
// methods MinimumCostPathWithEdgeReversalsTests proves correct. BruteForceDijkstra
// still takes LeetCode's own (n, edges) shape and builds its own BCL adjacency lists
// inside the measured call, deliberately without this repo's graph engine;
// ShortestPathDijkstra is handed the prepared ReversalGraph its hoisted overload
// takes, so graph construction is charged to [GlobalSetup] rather than to the search
// being measured.
[MemoryDiagnoser]
public class MinimumCostPathWithEdgeReversalsBenchmarks
{
    private const int Seed = 3650;
    private const int ExtraEdgesPerNode = 2;

    [Params(50, 500)]
    public int NodeCount;

    private int[][] _edges = null!;
    private ReversalGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        _edges = MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed: Seed);
        _graph = ReversalGraph.Build(NodeCount, _edges);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDijkstra() =>
        MinimumCostPathWithEdgeReversalsSolution.MinCostByBruteForceDijkstra(NodeCount, _edges);

    [Benchmark]
    public int ShortestPathDijkstra() =>
        MinimumCostPathWithEdgeReversalsSolution.MinCostByShortestPathDijkstra(_graph);
}
