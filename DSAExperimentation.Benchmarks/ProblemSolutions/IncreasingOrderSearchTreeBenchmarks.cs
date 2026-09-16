using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.IncreasingOrderSearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IncreasingOrderSearchTreeSolution's, the same methods
// IncreasingOrderSearchTreeTests proves correct. A hand-rolled recursive in-order walk
// (no repo primitive) threading a running tail node through recursive parameters and
// return values, vs. this repo's own InOrderTraversal/IInOrderHooks doing the identical
// Left=null/Right=tail relink through AsyncLocal-threaded state - the same
// genuinely-distinct-walk-mechanics comparison KthSmallestElementInABSTBenchmarks
// already makes for LC 230, not an asymptotic win (both are O(n)). [GlobalSetup] fixes
// the shuffled source values, but each [Benchmark] still rebuilds a fresh
// shuffle-inserted BST from them via this repo's own BinarySearchTree<int>, because
// both walks mutate the tree's Left/Right pointers in place and would otherwise
// corrupt a later iteration - the same per-invocation restore ConvertBSTToGreaterTree-
// Benchmarks does with its Clone.
[MemoryDiagnoser]
public class IncreasingOrderSearchTreeBenchmarks
{
    private int[] _shuffledValues = [];

    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _shuffledValues = SeededSequences.ShuffledOneTo(NodeCount, seed: 1);
    }

    [Benchmark(Baseline = true)]
    public int RecursiveRelink() =>
        IncreasingOrderSearchTreeSolution.IncreasingBstByRecursiveRelink(BuildTree())!.Value;

    [Benchmark]
    public int InOrderTraversalHooks() =>
        IncreasingOrderSearchTreeSolution.IncreasingBstByInOrderHooks(BuildTree())!.Value;

    private BinaryTreeNode<int> BuildTree()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _shuffledValues)
        {
            tree.Insert(value);
        }

        return tree.Root!;
    }
}
