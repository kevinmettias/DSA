using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumWeightedSubgraphWithTheRequiredPathsSolution's,
// the same methods MinimumWeightedSubgraphWithTheRequiredPathsTests proves correct.
// PerNodePointToPointSearch still takes LeetCode's own (n, edges) shape and builds
// its own BCL adjacency lists inside the measured call, querying every candidate
// meeting vertex with three fresh point-to-point searches; ReverseGraphDijkstra is
// handed the prepared RequiredPathsGraph its hoisted overload takes, so building
// both orientations is charged to [GlobalSetup] rather than to the three searches
// being measured.
[MemoryDiagnoser]
public class MinimumWeightedSubgraphWithTheRequiredPathsBenchmarks
{
    private const int RandomSeed = 2203;
    private const int ExtraEdgesPerNode = 3;
    private const int Src1 = 0;
    private const int Src2 = 1;

    private int[][] _edges = [];

    private RequiredPathsGraph _graph = null!;
    private int _dest;
    [Params(30, 150)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed: RandomSeed);
        _graph = RequiredPathsGraph.Build(NodeCount, _edges);
        _dest = NodeCount - 1;
    }

    [Benchmark(Baseline = true)]
    public long PerNodePointToPointSearch() =>
        MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByPerNodeSearch(
            NodeCount,
            _edges,
            new MinimumWeightedSubgraphWithTheRequiredPathsSolution.PathEndpoints(Src1, Src2, _dest));

    [Benchmark]
    public long ReverseGraphDijkstra() =>
        MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByReverseGraphDijkstra(
            _graph,
            new MinimumWeightedSubgraphWithTheRequiredPathsSolution.PathEndpoints(Src1, Src2, _dest));
}
