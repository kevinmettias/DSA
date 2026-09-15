using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToBuyApplesIISolution's, the same
// methods MinimumCostToBuyApplesIITests proves correct. Each arm is handed the
// same prepared AppleNetwork, built once in [GlobalSetup] from the random road
// network ApplesWorkloads generates, so graph construction is charged to setup
// and only the per-source shortest-path engine differs between arms
// (NetworkRecoveryPathwaysBenchmarks precedent).
[MemoryDiagnoser]
public class MinimumCostToBuyApplesIIBenchmarks
{
    private const int Seed = 3928; // LC problem number
    private const int ExtraRoadsPerShop = 2;

    private AppleNetwork _network = null!;

    [Params(30, 120)]
    public int ShopCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (prices, roads) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);
        _network = AppleNetwork.Build(ShopCount, prices, roads);
    }

    [Benchmark(Baseline = true)]
    public long[] BruteForceDijkstra() => MinimumCostToBuyApplesIISolution.MinCostsByBruteForceDijkstra(_network);

    [Benchmark]
    public long[] ReduceGraph() => MinimumCostToBuyApplesIISolution.MinCostsByReduceGraph(_network);
}
