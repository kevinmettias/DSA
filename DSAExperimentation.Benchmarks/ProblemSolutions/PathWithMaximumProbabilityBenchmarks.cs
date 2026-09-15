using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PathWithMaximumProbability;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathWithMaximumProbabilitySolution's, the same methods
// PathWithMaximumProbabilityTests proves correct. The textbook exhaustive walk over
// every source-to-target path (exponential in NodeCount - every incident edge branches
// the path count) against this repo's own ShortestPath.Dijkstra run over the same
// edges reweighted to -log(probability), the non-negative transform the solution
// documents, which turns "maximize a product" into Dijkstra's own "minimize a
// non-negative sum" with zero changes to Dijkstra itself.
//
// Both arms are handed the prepared ProbabilityGraph their hoisted overloads take, so
// building the graph is charged to [GlobalSetup] rather than to the search.
[MemoryDiagnoser]
public class PathWithMaximumProbabilityBenchmarks
{
    private const int RandomSeed = 1514; // LC problem number
    private const int ExtraEdgesPerNode = 2;

    private ProbabilityGraph _graph = null!;

    private int _target;
    [Params(10, 14)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _target = NodeCount - 1;
        var (edges, probabilities) = ProbabilityGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, RandomSeed);
        _graph = ProbabilityGraph.Build(NodeCount, edges, probabilities);
    }

    [Benchmark(Baseline = true)]
    public double ExhaustiveDfsOverEveryPath() =>
        PathWithMaximumProbabilitySolution.MaxProbabilityByExhaustiveDfs(_graph, start: 0, _target);

    [Benchmark]
    public double DijkstraOverNegativeLogWeights() =>
        PathWithMaximumProbabilitySolution.MaxProbabilityByDijkstra(_graph, start: 0, _target);
}
