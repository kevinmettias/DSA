using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountConnectedSubgraphsWithEvenNodeSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountConnectedSubgraphsWithEvenNodeSumSolution's,
// the same methods CountConnectedSubgraphsWithEvenNodeSumTests proves correct.
// Neither arm mutates nums/edges, so a single [GlobalSetup] build is enough -
// unlike GoodSubsequenceQueriesBenchmarks, there is no per-query state to reset
// between iterations.
[MemoryDiagnoser]
public class CountConnectedSubgraphsWithEvenNodeSumBenchmarks
{
    private const int Seed = 3910;

    private int[] _nums = [];

    private int[][] _edges = [];
    [Params(8, 13)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => (_nums, _edges) = EvenNodeSumGraphWorkloads.Build(NodeCount, seed: Seed);

    [Benchmark(Baseline = true)]
    public int BruteForceBfs() =>
        CountConnectedSubgraphsWithEvenNodeSumSolution.CountEvenSumSubgraphsByBruteForceBfs(_nums, _edges);

    [Benchmark]
    public int DisjointSet() =>
        CountConnectedSubgraphsWithEvenNodeSumSolution.CountEvenSumSubgraphsByDisjointSet(_nums, _edges);
}
