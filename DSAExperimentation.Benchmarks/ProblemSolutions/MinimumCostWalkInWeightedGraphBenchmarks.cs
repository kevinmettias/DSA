using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostWalkInWeightedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostWalkInWeightedGraphSolution's, the same
// methods MinimumCostWalkInWeightedGraphTests proves correct. The composed arm is
// handed a prebuilt WalkCostComponents - one Union-Find pass over every edge - so
// that one-time cost is charged to [GlobalSetup], not to the queries being
// measured.
[MemoryDiagnoser]
public class MinimumCostWalkInWeightedGraphBenchmarks
{
    private const int Seed = 3108;
    private const int QueryCount = 200;

    [Params(200, 2_000)]
    public int NodeCount;

    private int[][] _edges = null!;
    private int[][] _query = null!;
    private WalkCostComponents _components = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_edges, _query) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, seed: Seed);
        _components = WalkCostComponents.Build(NodeCount, _edges);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceWalk() =>
        MinimumCostWalkInWeightedGraphSolution.MinimumCostByBruteForceWalk(NodeCount, _edges, _query);

    [Benchmark]
    public int[] UnionFind() =>
        MinimumCostWalkInWeightedGraphSolution.MinimumCostByUnionFind(_components, _query);
}
