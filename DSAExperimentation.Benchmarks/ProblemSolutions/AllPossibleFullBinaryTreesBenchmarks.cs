using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AllPossibleFullBinaryTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllPossibleFullBinaryTreesSolution's, the same methods
// AllPossibleFullBinaryTreesTests proves correct. Mirrors
// UniqueBinarySearchTreesIIBenchmarks' naive-vs-Memoizer pairing for LC 95, keyed
// here by a single node count instead of a (start,end) range.
[MemoryDiagnoser]
public class AllPossibleFullBinaryTreesBenchmarks
{
    [Params(13, 19)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public int Naive() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByPlainRecursion(Nodes).Count;

    [Benchmark]
    public int Memoized() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByMemoizedNodeCount(Nodes).Count;
}
