using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniqueBinarySearchTreesIISolution's, the same
// methods UniqueBinarySearchTreesIITests proves correct. Mirrors
// UniqueBinarySearchTreesBenchmarks' tabulation-vs-Memoizer pairing for LC
// 96, the counting-only sibling of this problem.
[MemoryDiagnoser]
public class UniqueBinarySearchTreesIIBenchmarks
{
    [Params(8, 12)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public int PlainRecursion() => UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(Nodes).Count;

    [Benchmark]
    public int MemoizedRange() => UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(Nodes).Count;
}
