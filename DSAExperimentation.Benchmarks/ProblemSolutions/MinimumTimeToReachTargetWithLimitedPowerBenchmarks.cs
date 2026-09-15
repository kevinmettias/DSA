using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumTimeToReachTargetWithLimitedPowerSolution's, the same methods the
// coverage tests prove correct. The composed arm is handed the prepared
// PowerStateGraph its hoisted overload takes, so wiring the whole
// (node, remainingPower) state space is charged to [GlobalSetup] rather than
// to the search being measured (OpenTheLockBenchmarks precedent).
[MemoryDiagnoser]
public class MinimumTimeToReachTargetWithLimitedPowerBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int Seed = 3977;
    private const int Power = 30;

    private int[][] _edges = [];

    private int[] _cost = [];
    private PowerStateGraph _graph = null!;
    [Params(100, 500)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_edges, _cost) = PowerStateWorkloads.Build(NodeCount, Seed);
        _graph = PowerStateGraph.Build(NodeCount, _edges, Power, _cost);
    }

    [Benchmark(Baseline = true)]
    public long[] BclPriorityQueue() =>
        MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByBclPriorityQueue(
            (NodeCount, _edges, Power, _cost), source: 0, target: NodeCount - 1);

    [Benchmark]
    public long[] ReduceGraph() =>
        MinimumTimeToReachTargetWithLimitedPowerSolution.MinTimeMaxPowerByReduceGraph(
            _graph, source: 0, target: NodeCount - 1);
}
