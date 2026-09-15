using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestIntegerDivisibleByKSolution's, the same methods
// SmallestIntegerDivisibleByKTests proves correct - the textbook modular walk (a
// single int updated in place for up to k steps) against Reduce.Graph +
// DistanceMapReduceAlgebra over the remainder graph, where each node's single
// outgoing edge stands in for "append one more '1' digit". The graph is built once in
// [GlobalSetup] so construction is not charged to the measured method; both K values
// are coprime to 10 (odd, not a multiple of 5), so both arms walk the full distance to
// remainder 0 rather than short-circuiting on an immediate "-1".
[MemoryDiagnoser]
public class SmallestIntegerDivisibleByKBenchmarks
{
    private RemainderGraph _graph = null!;

    [Params(201, 5_001)]
    public int K { get; set; }

    [GlobalSetup]
    public void Setup() => _graph = RemainderGraph.Build(K);

    [Benchmark(Baseline = true)]
    public int ModularWalk() => SmallestIntegerDivisibleByKSolution.SmallestRepunitLengthByModularWalk(K);

    [Benchmark]
    public int ReduceGraphBfs() => SmallestIntegerDivisibleByKSolution.SmallestRepunitLengthByReduceGraph(_graph);
}
