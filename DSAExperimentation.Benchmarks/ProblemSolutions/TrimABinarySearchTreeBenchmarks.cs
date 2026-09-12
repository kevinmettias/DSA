using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TrimABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrimABinarySearchTreeSolution's, the same methods
// TrimABinarySearchTreeTests proves correct. Low/high span the tree's whole
// value range, so every node survives under both approaches - isolating the
// rebuild-vs-reattach cost itself rather than how much of the tree gets dropped.
[MemoryDiagnoser]
public class TrimABinarySearchTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

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

    [Benchmark(Baseline = true)]
    public int CollectAndRebuild() =>
        CountNodes(TrimABinarySearchTreeSolution.TrimByCollectAndRebuild(_root, 0, NodeCount - 1));

    [Benchmark]
    public int InPlaceTrim() =>
        CountNodes(TrimABinarySearchTreeSolution.TrimByInPlaceMutation(_root, 0, NodeCount - 1));

    // BinaryTreeNode<int> is internal, so a public [Benchmark] method can't return
    // it directly (CS0050) - this projects the result to a public int just to give
    // BenchmarkDotNet a return value, the same role WordLadderIIBenchmarks' .Count
    // plays per ARCHITECTURE.md §17.8. It is not a second copy of either strategy.
    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
