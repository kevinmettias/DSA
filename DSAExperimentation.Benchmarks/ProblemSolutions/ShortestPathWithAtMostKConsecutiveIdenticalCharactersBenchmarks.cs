using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution's, the same
// methods the coverage tests prove correct. The composed arm is handed the
// prepared ConsecutiveRunGraph its hoisted overload takes, so wiring the whole
// (node, runLength) state space is charged to [GlobalSetup] rather than to the
// search being measured (OpenTheLockBenchmarks precedent).
[MemoryDiagnoser]
public class ShortestPathWithAtMostKConsecutiveIdenticalCharactersBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int Seed = 3970;
    private const int K = 5;

    private int[][] _edges = [];

    private string _labels = "";
    private ConsecutiveRunGraph _graph = null!;
    [Params(100, 500)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_edges, _labels) = ConsecutiveRunWorkloads.Build(NodeCount, Seed);
        _graph = ConsecutiveRunGraph.Build(NodeCount, _edges, _labels, K);
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() =>
        ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByBclPriorityQueue(
            NodeCount, _edges, _labels, K);

    [Benchmark]
    public int ReduceGraph() =>
        ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByReduceGraph(_graph);
}
