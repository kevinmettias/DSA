using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InvertBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is InvertBinaryTreeSolution's, the same method
// InvertBinaryTreeSolutionTests proves correct. The original benchmark's two [Benchmark]
// arms were unimplemented stubs (each just returned the literal 1, ignoring the
// tree entirely), so there was nothing to preserve from them beyond the fact that
// this benchmark exists.
//
// The strategy mutates the tree it is handed, so each invocation clones the
// shared tree first (the FlattenBinaryTreeToLinkedList convention for a
// mutate-in-place problem) rather than re-inverting an already-inverted tree on
// every later call. Returns the inverted clone as object? rather than the internal
// BinaryTreeNode<int> - the accommodation ReverseLinkedList, SortList and friends
// make, since a public [Benchmark] method cannot name an internal return type
// (CS0050). It returned void before, which dropped the arm's only answer.
public class InvertBinaryTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(255, 65_535)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark]
    public object? RecursiveSwap() => InvertBinaryTreeSolution.InvertByRecursiveSwap(BinaryTrees.Clone(_root));
}
