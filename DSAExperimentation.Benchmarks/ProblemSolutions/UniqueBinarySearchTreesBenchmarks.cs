using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UniqueBinarySearchTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniqueBinarySearchTreesSolution's, the same
// methods UniqueBinarySearchTreesTests proves correct.
[MemoryDiagnoser]
public class UniqueBinarySearchTreesBenchmarks
{
    [Params(10, 16)]
    public int Nodes;

    [Benchmark(Baseline = true)]
    public int Tabulation() => UniqueBinarySearchTreesSolution.NumTreesByTabulation(Nodes);

    [Benchmark]
    public int MemoizedCatalan() => UniqueBinarySearchTreesSolution.NumTreesByMemoizedCatalan(Nodes);
}
