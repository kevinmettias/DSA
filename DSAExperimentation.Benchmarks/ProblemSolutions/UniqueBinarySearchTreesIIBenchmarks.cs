using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniqueBinarySearchTreesIISolution's, the same
// methods UniqueBinarySearchTreesIISolutionTests proves correct. Mirrors
// UniqueBinarySearchTreesBenchmarks' tabulation-vs-Memoizer pairing for LC
// 96, the counting-only sibling of this problem. Each arm returns the list of
// tree roots it built. Nodes stops at LC 95's n = 8.
public class UniqueBinarySearchTreesIIBenchmarks
{
    [Params(4, 8)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public List<BinaryTreeNode<int>?> PlainRecursion() => UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(Nodes);

    [Benchmark]
    public List<BinaryTreeNode<int>?> MemoizedRange() => UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(Nodes);
}
