using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Trim a Binary Search Tree (LC 669): collecting every in-range value via a full
// in-order walk and reinserting them one at a time into a fresh
// BinarySearchTree<int> - re-paying an O(n) descent per insert, O(n^2) worst case
// on the already-sorted sequence a skewed input produces - vs. the O(n)
// single-pass recursive trim that reassigns BinaryTreeNode<int>.Left/Right in
// place with no rebuild at all. Low/high span the tree's whole value range, so
// every node survives under both approaches - isolating the rebuild-vs-reattach
// cost itself rather than how much of the tree gets dropped.
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
    public int CollectAndRebuild()
    {
        var kept = new List<int>();
        CollectInRange(_root, 0, NodeCount - 1, kept);

        var tree = new BinarySearchTree<int>();

        foreach (var value in kept)
        {
            tree.Insert(value);
        }

        return CountNodes(tree.Root);
    }

    private static void CollectInRange(BinaryTreeNode<int>? node, int low, int high, List<int> kept)
    {
        if (node is null)
        {
            return;
        }

        CollectInRange(node.Left, low, high, kept);

        if (node.Value >= low && node.Value <= high)
        {
            kept.Add(node.Value);
        }

        CollectInRange(node.Right, low, high, kept);
    }

    [Benchmark]
    public int InPlaceTrim()
    {
        var trimmed = Trim(_root, 0, NodeCount - 1);
        return CountNodes(trimmed);
    }

    private static BinaryTreeNode<int>? Trim(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return null;
        }

        if (node.Value < low)
        {
            return Trim(node.Right, low, high);
        }

        if (node.Value > high)
        {
            return Trim(node.Left, low, high);
        }

        node.Left = Trim(node.Left, low, high);
        node.Right = Trim(node.Right, low, high);
        return node;
    }

    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
