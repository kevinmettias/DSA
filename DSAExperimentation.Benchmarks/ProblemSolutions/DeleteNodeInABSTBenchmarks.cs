using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DeleteNodeInABST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteNodeInABSTSolution's, the same methods
// DeleteNodeInABSTSolutionTests proves correct. TryDelete removes the key in place, so a
// fresh tree is built from the same shuffled insertion order on every call rather than
// hoisting one shared instance into [GlobalSetup]. The build is timed on purpose: the
// baseline builds its own new tree and leaves its input alone, but it pays the same
// build, so every arm pays the same cost - and BinarySearchTree<int> is filled only by
// Insert, so re-inserting is the cheapest fresh copy it has. The order is shuffled so
// height stays close to O(log n) instead of the degenerate O(n) ascending-insertion
// case, the same convention KthSmallestElementInABSTBenchmarks already uses. Each arm
// returns the tree with the key removed as object?, since a public [Benchmark] method
// cannot name the internal BinarySearchTree<int> (CS0050).
public class DeleteNodeInABSTBenchmarks
{

    private int[] _insertionOrder = [];

    private int _target;
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _insertionOrder = SeededSequences.ShuffledZeroTo(NodeCount, seed: 1);
        _target = NodeCount / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public object? CollectFilterRebuild() =>
        DeleteNodeInABSTSolution.DeleteByCollectFilterRebuild(BuildTree(_insertionOrder), _target);

    [Benchmark]
    public object? BinarySearchTreeTryDelete() =>
        DeleteNodeInABSTSolution.DeleteByBinarySearchTreeDelete(BuildTree(_insertionOrder), _target);

    private static BinarySearchTree<int> BuildTree(int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        return tree;
    }
}
