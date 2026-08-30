using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Sum of BST (LC 938): a full-tree scan that checks every node against
// [low, high] regardless of ordering (O(n), the same complexity a plain binary
// tree would force) vs. the BST-ordering-pruned walk RangeSumOfBSTTests itself
// uses, which skips an entire subtree the moment a node proves it can't contain
// anything in range. _low/_high are deliberately narrow and near the low end of
// the value domain, so pruning discards most of a large tree instead of merely
// skipping a few leaves.
[MemoryDiagnoser]
public class RangeSumOfBSTBenchmarks
{
    private const int Low = 0;
    private const int High = 20;

    [Params(500, 20_000)]
    public int Length;

    private BinaryTreeNode<int>? _root;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, Length).ToArray();
        var random = new Random(938);

        // Fisher-Yates shuffle before insertion, so BinarySearchTree.Insert builds
        // an expected-O(log n)-height tree instead of the O(n)-height degenerate
        // chain ascending input would force.
        for (var i = values.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
        }

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        _root = tree.Root;
    }

    [Benchmark(Baseline = true)]
    public int FullTreeScan() => FullScan(_root);

    [Benchmark]
    public int BstPrunedWalk() => PrunedRangeSum(_root);

    private static int FullScan(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        var contribution = node.Value >= Low && node.Value <= High ? node.Value : 0;
        return contribution + FullScan(node.Left) + FullScan(node.Right);
    }

    private static int PrunedRangeSum(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        if (node.Value < Low)
        {
            return PrunedRangeSum(node.Right);
        }

        if (node.Value > High)
        {
            return PrunedRangeSum(node.Left);
        }

        return node.Value + PrunedRangeSum(node.Left) + PrunedRangeSum(node.Right);
    }
}
