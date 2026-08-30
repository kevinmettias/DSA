using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Recover Binary Search Tree (LC 99): a hand-rolled recursive in-order collect-then
// -scan baseline vs. this repo's InOrderTraversal/IInOrderHooks walking the same
// BinaryTreeNode<int> tree with no intermediate list allocation. Both benchmarks
// only locate the swapped pair (they don't write the values back), so the fixture's
// single seeded corruption stays valid across every iteration of a run.
[MemoryDiagnoser]
public class RecoverBinarySearchTreeBenchmarks
{
    [Params(100, 5_000)]
    public int Size;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BuildCorruptedBst(Size);

    [Benchmark(Baseline = true)]
    public (int First, int Second) ManualRecursiveScan()
    {
        var values = new List<BinaryTreeNode<int>>();
        CollectInOrder(_root, values);

        BinaryTreeNode<int>? first = null;
        BinaryTreeNode<int>? second = null;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i - 1].Value > values[i].Value)
            {
                first ??= values[i - 1];
                second = values[i];
            }
        }

        return (first!.Value, second!.Value);
    }

    [Benchmark]
    public (int First, int Second) InOrderTraversalHooks()
    {
        State.Prev.Value = null;
        State.First.Value = null;
        State.Second.Value = null;

        InOrderTraversal.Walk<int, ScanHooks>(_root);

        return (State.First.Value!.Value, State.Second.Value!.Value);
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<BinaryTreeNode<int>> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node);
        CollectInOrder(node.Right, values);
    }

    private readonly struct ScanHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Prev.Value is { } prev && prev.Value > node.Value)
            {
                State.First.Value ??= prev;
                State.Second.Value = node;
            }

            State.Prev.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Prev = new();
        public static readonly AsyncLocal<BinaryTreeNode<int>?> First = new();
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Second = new();
    }

    // A balanced BST over 0..size-1 with its min and max values swapped - one
    // non-adjacent violation pair for the detection loop to find, independent of size.
    private static BinaryTreeNode<int> BuildCorruptedBst(int size)
    {
        var values = Enumerable.Range(0, size).ToArray();
        var root = BuildBalanced(values, 0, size - 1)!;

        var min = FindMin(root);
        var max = FindMax(root);
        (min.Value, max.Value) = (max.Value, min.Value);

        return root;
    }

    private static BinaryTreeNode<int>? BuildBalanced(int[] values, int low, int high)
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

    private static BinaryTreeNode<int> FindMin(BinaryTreeNode<int> node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }

    private static BinaryTreeNode<int> FindMax(BinaryTreeNode<int> node)
    {
        while (node.Right is not null)
        {
            node = node.Right;
        }

        return node;
    }
}
