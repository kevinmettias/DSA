using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TrimABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrimABinarySearchTreeSolution's, the same methods
// TrimABinarySearchTreeSolutionTests proves correct. The tree is a BST of 1..NodeCount
// inserted in seeded shuffled order, so its height stays near log n, and low/high
// span the whole value range, so every node survives and neither arm changes the tree
// - each invocation trims the same tree. That isolates what the strategies differ in:
// the recursion visits every node to find that nothing needs dropping, while the
// boundary walk follows only the leftmost and rightmost paths.
public class TrimABinarySearchTreeBenchmarks
{
    private const int ShuffleSeed = 669; // LC problem number

    private BinaryTreeNode<int> _root = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in SeededSequences.ShuffledOneTo(NodeCount, ShuffleSeed))
        {
            tree.Insert(value);
        }

        _root = tree.Root!;
    }

    // Each arm returns the trimmed tree's root, as object because BinaryTreeNode<int>
    // is internal and a public [Benchmark] method can't name it (CS0050).
    [Benchmark(Baseline = true)]
    public object? InPlaceTrim() =>
        TrimABinarySearchTreeSolution.TrimByInPlaceMutation(_root, 1, NodeCount);

    [Benchmark]
    public object? IterativeBoundaryWalk() =>
        TrimABinarySearchTreeSolution.TrimByIterativeBoundaryWalk(_root, 1, NodeCount);
}
