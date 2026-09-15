using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DeleteNodeInABST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteNodeInABSTSolution's, the same methods
// DeleteNodeInABSTTests proves correct. Each arm deletes from the tree, so a fresh
// tree is built from the same shuffled insertion order on every call rather than
// hoisting one shared instance into [GlobalSetup] - shuffled so height stays close
// to O(log n) instead of the degenerate O(n) ascending-insertion case, the same
// convention KthSmallestElementInABSTBenchmarks already uses.
[MemoryDiagnoser]
public class DeleteNodeInABSTBenchmarks
{

    private int[] _insertionOrder = [];

    private int _target;
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _insertionOrder = values;
        _target = NodeCount / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public int CollectFilterRebuild() =>
        DeleteNodeInABSTSolution.DeleteByCollectFilterRebuild(BuildTree(_insertionOrder), _target).Count;

    [Benchmark]
    public int BinarySearchTreeTryDelete() =>
        DeleteNodeInABSTSolution.DeleteByBinarySearchTreeDelete(BuildTree(_insertionOrder), _target).Count;

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
