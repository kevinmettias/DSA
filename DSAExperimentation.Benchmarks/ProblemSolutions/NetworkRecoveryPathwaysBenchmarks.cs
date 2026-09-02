using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NetworkRecoveryPathways;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NetworkRecoveryPathwaysSolution's, the same
// methods NetworkRecoveryPathwaysTests proves correct. Each arm is handed the
// same prepared RecoveryNetwork, built once in [GlobalSetup] from the random
// DAG NetworkRecoveryWorkloads generates, so graph construction is charged to
// setup and only the per-threshold shortest-path engine differs between arms
// (OpenTheLockBenchmarks precedent).
[MemoryDiagnoser]
public class NetworkRecoveryPathwaysBenchmarks
{
    private const int Seed = 3620; // LC problem number
    private const int ExtraEdgesPerNode = 3;
    private const long Budget = 50_000_000_000L; // generous - keeps most probes feasible

    [Params(300, 3000)]
    public int NodeCount;

    private RecoveryNetwork _network = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (edges, online) = NetworkRecoveryWorkloads.Build(NodeCount, ExtraEdgesPerNode, Seed);
        _network = RecoveryNetwork.Build(NodeCount, edges, online);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDijkstra() =>
        NetworkRecoveryPathwaysSolution.FindMaxPathScoreByBruteForceDijkstra(_network, Budget);

    [Benchmark]
    public int ReduceGraph() =>
        NetworkRecoveryPathwaysSolution.FindMaxPathScoreByReduceGraph(_network, Budget);
}
