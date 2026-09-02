using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InvertBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is InvertBinaryTreeSolution's, the same method
// InvertBinaryTreeTests proves correct. The original benchmark's two [Benchmark]
// arms were unimplemented stubs (each just returned the literal 1, ignoring the
// tree entirely), so there was nothing to preserve from them beyond the fact that
// this benchmark exists.
//
// The strategy mutates the tree it is handed, so each invocation clones the
// shared tree first (the FlattenBinaryTreeToLinkedList convention for a
// mutate-in-place problem) rather than re-inverting an already-inverted tree on
// every later call. Returns void, not BinaryTreeNode<int> - the node type is
// internal, so a public [Benchmark] method cannot name it as a return type
// (CS0050).
[MemoryDiagnoser]
public class InvertBinaryTreeBenchmarks
{
    [Params(255, 65_535)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark]
    public void RecursiveSwap() => InvertBinaryTreeSolution.InvertByRecursiveSwap(Clone(_root));

    private static BinaryTreeNode<int> Clone(BinaryTreeNode<int> node) => new(node.Value)
    {
        Left = node.Left is null ? null : Clone(node.Left),
        Right = node.Right is null ? null : Clone(node.Right),
    };
}
