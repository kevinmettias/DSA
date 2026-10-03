using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TrimABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrimABinarySearchTreeSolution's, the same methods
// TrimABinarySearchTreeSolutionTests proves correct. Low/high span the tree's whole
// value range, so every node survives under both approaches - isolating the
// rebuild-vs-reattach cost itself rather than how much of the tree gets dropped.
public class TrimABinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = new BinarySearchTree<int>();

        for (var i = 0; i < NodeCount; i++)
        {
            tree.Insert(i);
        }

        _root = tree.Root!;
    }

    // Each arm returns the trimmed tree's root, as object because BinaryTreeNode<int>
    // is internal and a public [Benchmark] method can't name it (CS0050).
    [Benchmark(Baseline = true)]
    public object? CollectAndRebuild() =>
        TrimABinarySearchTreeSolution.TrimByCollectAndRebuild(_root, 0, NodeCount - 1);

    [Benchmark]
    public object? InPlaceTrim() =>
        TrimABinarySearchTreeSolution.TrimByInPlaceMutation(_root, 0, NodeCount - 1);
}
