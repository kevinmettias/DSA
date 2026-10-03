using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FlattenBinaryTreeToLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenBinaryTreeToLinkedListSolution's, the same
// methods FlattenBinaryTreeToLinkedListSolutionTests proves correct. The original
// benchmark's two [Benchmark] arms were unimplemented stubs (each just returned
// the literal 1, ignoring the tree entirely), so there was nothing to preserve
// from them beyond the fact that this problem wants two arms.
//
// Both strategies mutate the tree they are handed, so each [Benchmark] clones the
// shared tree first (the ConvertBSTToGreaterTree convention for a mutate-in-place
// problem) rather than re-flattening an already-flattened tree on every later
// invocation. Each arm returns the flattened clone as object? rather than the
// internal BinaryTreeNode<int> - the accommodation ReverseLinkedList, SortList and
// friends make, since a public [Benchmark] method cannot name an internal return
// type (CS0050). It returned void before, which dropped the only thing the two arms
// could be compared on.
//
// LC 114 caps the tree at 2000 nodes valued -100..100, so the larger NodeCount is that
// cap and the complete tree's level order cycles through those values.
public class FlattenBinaryTreeToLinkedListBenchmarks
{
    private const int LowestValue = -100;

    // How many values -100..100 holds.
    private const int ValueCount = 201;

    private BinaryTreeNode<int> _root = null!;

    [Params(500, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = Enumerable.Range(0, NodeCount).Select(ValueAt).ToArray();
        _root = BinaryTrees.Complete(levelOrder);
    }

    private static int ValueAt(int index) => LowestValue + (index % ValueCount);

    [Benchmark(Baseline = true)]
    public object? RecursiveSplice()
    {
        var root = BinaryTrees.Clone(_root);
        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(root);

        return root;
    }

    [Benchmark]
    public object? TopDownPreorderRelink()
    {
        var root = BinaryTrees.Clone(_root);
        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(root);

        return root;
    }
}
