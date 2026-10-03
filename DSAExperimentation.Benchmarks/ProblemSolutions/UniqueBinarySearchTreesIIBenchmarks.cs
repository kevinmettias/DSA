using DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniqueBinarySearchTreesIISolution's, the same
// methods UniqueBinarySearchTreesIISolutionTests proves correct. Mirrors
// UniqueBinarySearchTreesBenchmarks' tabulation-vs-Memoizer pairing for LC
// 96, the counting-only sibling of this problem. Each arm returns the list of
// tree roots it built, as object because BinaryTreeNode<int> is internal. Nodes
// stops at LC 95's n = 8.
public class UniqueBinarySearchTreesIIBenchmarks
{
    [Params(4, 8)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public object? PlainRecursion() => UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(Nodes);

    [Benchmark]
    public object? MemoizedRange() => UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(Nodes);
}
