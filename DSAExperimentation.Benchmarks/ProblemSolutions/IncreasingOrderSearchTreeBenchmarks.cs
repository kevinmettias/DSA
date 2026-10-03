using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.IncreasingOrderSearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IncreasingOrderSearchTreeSolution's, the same methods
// IncreasingOrderSearchTreeSolutionTests proves correct. A hand-rolled recursive in-order walk
// (no repo primitive) threading a running tail node through recursive parameters and
// return values, vs. this repo's own InOrderTraversal/IInOrderHooks doing the identical
// Left=null/Right=tail relink through a hook that carries the tail - the same
// genuinely-distinct-walk-mechanics comparison KthSmallestElementInABSTBenchmarks
// already makes for LC 230, not an asymptotic win (both are O(n)). [GlobalSetup]
// inserts the shuffled source values into this repo's own BinarySearchTree<int> once,
// and each [Benchmark] clones that tree, because both walks mutate the tree's Left/Right
// pointers in place and would otherwise corrupt a later iteration - the same
// per-invocation restore ConvertBSTToGreaterTreeBenchmarks does with its Clone. The
// clone is timed on purpose, so every arm pays the same O(n) copy rather than the
// O(n log n) of re-inserting every value. Each arm returns the relinked chain's root as
// object?, since a public [Benchmark] method cannot name the internal
// BinaryTreeNode<int> (CS0050).
public class IncreasingOrderSearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in SeededSequences.ShuffledOneTo(NodeCount, seed: 1))
        {
            tree.Insert(value);
        }

        _root = tree.Root!;
    }

    [Benchmark(Baseline = true)]
    public object? RecursiveRelink() =>
        IncreasingOrderSearchTreeSolution.IncreasingBstByRecursiveRelink(Clone(_root));

    [Benchmark]
    public object? InOrderTraversalHooks() =>
        IncreasingOrderSearchTreeSolution.IncreasingBstByInOrderHooks(Clone(_root));

    private static BinaryTreeNode<int> Clone(BinaryTreeNode<int> node) => new(node.Value)
    {
        Left = node.Left is null ? null : Clone(node.Left),
        Right = node.Right is null ? null : Clone(node.Right),
    };
}
