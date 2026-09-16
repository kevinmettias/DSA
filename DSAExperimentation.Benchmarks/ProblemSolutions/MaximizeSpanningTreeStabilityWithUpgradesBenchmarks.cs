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

    private int _upgrades;
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (edges, upgrades) = StabilityGraphWorkloads.Build(NodeCount, seed: Seed);
        _graph = StabilityGraph.Build(NodeCount, edges);
        _upgrades = upgrades;
    }

    [Benchmark(Baseline = true)]
    public int ArrayUnionFind() =>
        MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByArrayUnionFind(_graph, _upgrades);

    [Benchmark]
    public int DisjointSet() =>
        MaximizeSpanningTreeStabilityWithUpgradesSolution.MaxStabilityByDisjointSet(_graph, _upgrades);
}
