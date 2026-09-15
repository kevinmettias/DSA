using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FlattenBinaryTreeToLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenBinaryTreeToLinkedListSolution's, the same
// methods FlattenBinaryTreeToLinkedListTests proves correct. The original
// benchmark's two [Benchmark] arms were unimplemented stubs (each just returned
// the literal 1, ignoring the tree entirely), so there was nothing to preserve
// from them beyond the fact that this problem wants two arms.
//
// Both strategies mutate the tree they are handed, so each [Benchmark] clones the
// shared tree first (the ConvertBSTToGreaterTree convention for a mutate-in-place
// problem) rather than re-flattening an already-flattened tree on every later
// invocation. Returns void, not BinaryTreeNode<int> - the node type is internal,
// so a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class FlattenBinaryTreeToLinkedListBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public void RecursiveSplice() =>
        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(Clone(_root));

    [Benchmark]
    public void TopDownPreorderRelink() =>
        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(Clone(_root));

    private static BinaryTreeNode<int> Clone(BinaryTreeNode<int> node) => new(node.Value)
    {
        Left = node.Left is null ? null : Clone(node.Left),
        Right = node.Right is null ? null : Clone(node.Right),
    };
}
