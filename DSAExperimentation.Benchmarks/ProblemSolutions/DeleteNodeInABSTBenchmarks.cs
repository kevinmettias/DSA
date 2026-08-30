using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Delete Node in a BST (LC 450): a naive approach many first solutions reach for -
// collect every value via an in-order walk, filter the target out, and rebuild a
// fresh balanced BST from what remains (touches all n nodes on every delete) - vs.
// this repo's own BinarySearchTree<int>.TryDelete, which walks only the O(h) root-
// to-target path and reattaches just the affected subtree (in-order-successor
// promotion for the two-child case, see BinarySearchTree.cs's own
// DeleteFoundNode), never touching an unrelated node. Both benchmarks build their
// own input tree from the same shuffled insertion order so height stays close to
// O(log n) instead of the degenerate O(n) ascending-insertion case, the same
// convention KthSmallestElementInABSTBenchmarks already uses.
[MemoryDiagnoser]
public class DeleteNodeInABSTBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _insertionOrder = null!;
    private int _target;

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
        _target = NodeCount / 2;
    }

    [Benchmark(Baseline = true)]
    public int CollectFilterRebuild()
    {
        var root = BuildManual(_insertionOrder);

        var sorted = new List<int>();
        CollectInOrder(root, sorted);
        sorted.Remove(_target);

        return CountNodes(BuildBalanced(sorted, 0, sorted.Count - 1));
    }

    [Benchmark]
    public int BinarySearchTreeTryDelete()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _insertionOrder)
        {
            tree.Insert(value);
        }

        tree.TryDelete(_target);
        return tree.Count;
    }

    private static BinaryTreeNode<int> BuildManual(int[] values)
    {
        var root = new BinaryTreeNode<int>(values[0]);

        for (var i = 1; i < values.Length; i++)
        {
            InsertManual(root, values[i]);
        }

        return root;
    }

    private static void InsertManual(BinaryTreeNode<int> root, int value)
    {
        var node = root;

        while (true)
        {
            if (value < node.Value)
            {
                if (node.Left is null)
                {
                    node.Left = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right is null)
                {
                    node.Right = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Right;
            }
        }
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node.Value);
        CollectInOrder(node.Right, values);
    }

    private static BinaryTreeNode<int>? BuildBalanced(List<int> values, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / 2);

        return new BinaryTreeNode<int>(values[mid])
        {
            Left = BuildBalanced(values, low, mid - 1),
            Right = BuildBalanced(values, mid + 1, high),
        };
    }

    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
