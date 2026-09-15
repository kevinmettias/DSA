using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumTimeToVisitDisappearingNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToVisitDisappearingNodesSolution's, the
// same methods MinimumTimeToVisitDisappearingNodesTests proves correct. Each arm
// is handed a prebuilt TimedAdjacency, so adjacency-list construction is charged
// to [GlobalSetup] rather than to the search being measured.
[MemoryDiagnoser]
public class MinimumTimeToVisitDisappearingNodesBenchmarks
{
    private const int Seed = 3112;

    private TimedAdjacency _graph = null!;

    private int[] _disappear = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (edges, disappear) = DisappearingNodesWorkloads.Build(NodeCount, seed: Seed);
        _graph = TimedAdjacency.Build(NodeCount, edges);
        _disappear = disappear;
    }

    [Benchmark(Baseline = true)]
    public int[] DijkstraQueue() =>
        MinimumTimeToVisitDisappearingNodesSolution.MinimumTimesByDijkstraQueue(_graph, _disappear);

    [Benchmark]
    public int[] PriorityHeap() =>
        MinimumTimeToVisitDisappearingNodesSolution.MinimumTimesByPriorityHeap(_graph, _disappear);
}
