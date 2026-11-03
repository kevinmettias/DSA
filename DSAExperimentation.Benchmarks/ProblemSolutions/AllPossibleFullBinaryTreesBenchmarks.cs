using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.AllPossibleFullBinaryTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllPossibleFullBinaryTreesSolution's, the same methods
// AllPossibleFullBinaryTreesSolutionTests proves correct. Mirrors
// UniqueBinarySearchTreesIIBenchmarks' naive-vs-Memoizer pairing for LC 95, keyed
// here by a single node count instead of a (start,end) range. Each arm returns the
// list of tree roots itself.
public class AllPossibleFullBinaryTreesBenchmarks
{
    [Params(13, 19)]
    public int Nodes { get; set; }

    [Benchmark(Baseline = true)]
    public List<BinaryTreeNode<int>?> Naive() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByPlainRecursion(Nodes);

    [Benchmark]
    public List<BinaryTreeNode<int>?> Memoized() =>
        AllPossibleFullBinaryTreesSolution.AllPossibleFullBinaryTreesByMemoizedNodeCount(Nodes);
}
