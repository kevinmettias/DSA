using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PathWithMaximumProbability;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathWithMaximumProbabilitySolution's, the same methods
// PathWithMaximumProbabilitySolutionTests proves correct. The textbook exhaustive walk over
// every source-to-target path (exponential in the node count - every incident edge
// branches the path count) against this repo's own ShortestPath.Dijkstra run over the
// same edges reweighted to -log(probability), the non-negative transform the solution
// documents, which turns "maximize a product" into Dijkstra's own "minimize a
// non-negative sum" with zero changes to Dijkstra itself.
//
// Both arms are handed the prepared ProbabilityGraph their hoisted overloads take, so
// building the graph is charged to [GlobalSetup] rather than to the search.
//
// Sizes are per arm. The exhaustive walk stops at 14 nodes; Dijkstra runs on to 5,000,
// where this workload's three edges per node stay inside LC 1514's bound of 2 * 10^4
// edges. The two are compared at the sizes both run.
public class PathWithMaximumProbabilityBenchmarks
{
    private const int RandomSeed = 1514; // LC problem number
    private const int ExtraEdgesPerNode = 2;

    private Dictionary<int, ProbabilityGraph> _graphBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> DijkstraSizes => [.. BaselineSizes, 500, 5_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _graphBySize = DijkstraSizes.ToDictionary(nodeCount => nodeCount, BuildGraph);

    private static ProbabilityGraph BuildGraph(int nodeCount)
    {
        var (edges, probabilities) = ProbabilityGraphWorkloads.Build(nodeCount, ExtraEdgesPerNode, RandomSeed);

        return ProbabilityGraph.Build(nodeCount, edges, probabilities);
    }

    // Both arms search from node 0 to the last node, which the workload guarantees is connected to it.
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public double ExhaustiveDfsOverEveryPath(int nodeCount) =>
        PathWithMaximumProbabilitySolution.MaxProbabilityByExhaustiveDfs(
            _graphBySize[nodeCount], start: 0, nodeCount - 1);

    [Benchmark]
    [ArgumentsSource(nameof(DijkstraSizes))]
    public double DijkstraOverNegativeLogWeights(int nodeCount) =>
        PathWithMaximumProbabilitySolution.MaxProbabilityByDijkstra(
            _graphBySize[nodeCount], start: 0, nodeCount - 1);
}
