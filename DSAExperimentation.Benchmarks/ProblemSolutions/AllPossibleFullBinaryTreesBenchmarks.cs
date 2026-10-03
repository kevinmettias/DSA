using DSAExperimentation.LeetCode.AllPossibleFullBinaryTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllPossibleFullBinaryTreesSolution's, the same methods
// AllPossibleFullBinaryTreesSolutionTests proves correct. Mirrors
// UniqueBinarySearchTreesIIBenchmarks' naive-vs-Memoizer pairing for LC 95, keyed
// here by a single node count instead of a (start,end) range. Each arm returns the
// list of tree roots itself as object?, since a public [Benchmark] method cannot
// name the internal BinaryTreeNode<int> (CS0050).
public class AllPossibleFullBinaryTreesBenchmarks
{
    [Params(13, 19)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public object? Naive() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByPlainRecursion(Nodes);

    [Benchmark]
    public object? Memoized() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByMemoizedNodeCount(Nodes);
}
