using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumTimeForKConnectedComponents;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeForKConnectedComponentsSolution's, the same
// methods MinimumTimeForKConnectedComponentsTests proves correct.
[MemoryDiagnoser]
public class MinimumTimeForKConnectedComponentsBenchmarks
{
    private const int Seed = 3608;

    private int[][] _edges = [];

    private int _k;
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _edges = KConnectedComponentsWorkloads.BuildEdges(NodeCount, Seed);
        _k = Math.Max(1, NodeCount / 2);
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchUnionFind() =>
        MinimumTimeForKConnectedComponentsSolution.MinTimeByBinarySearchUnionFind(NodeCount, _edges, _k);

    [Benchmark]
    public int DescendingUnionFind() =>
        MinimumTimeForKConnectedComponentsSolution.MinTimeByDescendingUnionFind(NodeCount, _edges, _k);
}
