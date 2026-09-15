using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximizeSpanningTreeStabilityWithUpgradesSolution's, the same methods
// MaximizeSpanningTreeStabilityWithUpgradesTests proves correct. Each arm is
// handed a prebuilt StabilityGraph, so parsing edges into must/optional buckets is
// charged to [GlobalSetup] rather than the binary search being measured.
[MemoryDiagnoser]
public class MaximizeSpanningTreeStabilityWithUpgradesBenchmarks
{
    private const int Seed = 3600;

    private StabilityGraph _graph = null!;

    private int _k;
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (edges, k) = StabilityGraphWorkloads.Build(NodeCount, seed: Seed);
        _graph = StabilityGraph.Build(NodeCount, edges);
        _k = k;
    }

    [Benchmark(Baseline = true)]
    public int ArrayUnionFind() =>
        MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByArrayUnionFind(_graph, _k);

    [Benchmark]
    public int DisjointSet() =>
        MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByDisjointSet(_graph, _k);
}
