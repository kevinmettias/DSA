using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumTimeToReachDestinationInDirectedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumTimeToReachDestinationInDirectedGraphSolution's, the same methods
// MinimumTimeToReachDestinationInDirectedGraphTests proves correct. Each arm is
// handed a prebuilt TimeWindowAdjacency, so adjacency-list construction is
// charged to [GlobalSetup] rather than the search being measured.
[MemoryDiagnoser]
public class MinimumTimeToReachDestinationInDirectedGraphBenchmarks
{
    private const int Seed = 3604;

    private TimeWindowAdjacency _graph = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var edges = TimeWindowGraphWorkloads.Build(NodeCount, seed: Seed);
        _graph = TimeWindowAdjacency.Build(NodeCount, edges);
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() =>
        MinimumTimeToReachDestinationInDirectedGraphSolution.MinimumTimeByBclPriorityQueue(_graph);

    [Benchmark]
    public int Heap() =>
        MinimumTimeToReachDestinationInDirectedGraphSolution.MinimumTimeByHeap(_graph);
}
