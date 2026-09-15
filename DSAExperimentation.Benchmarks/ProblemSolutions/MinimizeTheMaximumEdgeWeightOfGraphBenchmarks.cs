using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeTheMaximumEdgeWeightOfGraphSolution's, the
// same methods MinimizeTheMaximumEdgeWeightOfGraphTests proves correct. Each arm
// is handed the prepared EdgeWeightGraph its hoisted overload takes, so building
// the reversed adjacency is charged to [GlobalSetup] rather than either
// feasibility search.
[MemoryDiagnoser]
public class MinimizeTheMaximumEdgeWeightOfGraphBenchmarks
{
    private const int Seed = 3419;
    private const int ExtraEdgesPerNode = 2;
    private const int Threshold = 4;

    private EdgeWeightGraph _graph = null!;

    [Params(100, 1000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var edges = EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);
        _graph = EdgeWeightGraph.Build(NodeCount, edges);
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchBfs() =>
        MinimizeTheMaximumEdgeWeightOfGraphSolution.MinMaxWeightByBinarySearchBfs(_graph, Threshold);

    [Benchmark]
    public int ReduceGraphBinarySearch() =>
        MinimizeTheMaximumEdgeWeightOfGraphSolution.MinMaxWeightByReduceGraphBinarySearch(_graph, Threshold);
}
