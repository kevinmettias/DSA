using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Insert into a Binary Search Tree (LC 701): a naive approach many first solutions
// reach for - collect the existing tree's values via an in-order walk, insert the
// new value into that sorted list, and rebuild a fresh balanced BST from the whole
// result (touches all n nodes on every insert) - vs. this repo's own
// BinarySearchTree<int>.Insert, which walks only the O(h) root-to-empty-slot path
// and attaches one new leaf, never touching an unrelated node. Both benchmarks
// build their own input tree from the same shuffled insertion order so height
// stays close to O(log n) instead of the degenerate O(n) ascending-insertion case,
// the same convention DeleteNodeInABSTBenchmarks already uses.
[MemoryDiagnoser]
public class InsertIntoABinarySearchTreeBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _insertionOrder = null!;
    private int _newValue;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).Select(v => v * 2).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _insertionOrder = values;
        _newValue = 1; // odd, so it always lands as a brand-new key between two existing even keys
    }

    [Benchmark(Baseline = true)]
    public int CollectSortInsertRebuild()
    {
        var root = BuildManual(_insertionOrder);

        var sorted = new List<int>();
        CollectInOrder(root, sorted);

        var insertAt = sorted.BinarySearch(_newValue);
        sorted.Insert(insertAt < 0 ? ~insertAt : insertAt, _newValue);

        return CountNodes(BuildBalanced(sorted, 0, sorted.Count - 1));
    }

    [Benchmark]
    public int BinarySearchTreeInsert()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _insertionOrder)
        {
            tree.Insert(value);
        }

        tree.Insert(_newValue);
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
