using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Absolute Difference in BST (LC 530): a hand-rolled recursive in-order
// collect-then-scan baseline vs. this repo's InOrderTraversal/IInOrderHooks walking
// the same BinaryTreeNode<int> tree with no intermediate list allocation, the same
// pairing RecoverBinarySearchTreeBenchmarks already uses.
[MemoryDiagnoser]
public class MinimumAbsoluteDifferenceInBSTBenchmarks
{
    [Params(100, 5_000)]
    public int Size;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BuildBalancedBst(Size);

    [Benchmark(Baseline = true)]
    public int ManualRecursiveScan()
    {
        var values = new List<int>();
        CollectInOrder(_root, values);

        var minDiff = int.MaxValue;
        for (var i = 1; i < values.Count; i++)
        {
            minDiff = Math.Min(minDiff, values[i] - values[i - 1]);
        }

        return minDiff;
    }

    [Benchmark]
    public int InOrderTraversalHooks()
    {
        State.Prev.Value = null;
        State.MinDiff.Value = int.MaxValue;

        InOrderTraversal.Walk<int, DiffHooks>(_root);

        return State.MinDiff.Value;
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

    private readonly struct DiffHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Prev.Value is { } prev)
            {
                State.MinDiff.Value = Math.Min(State.MinDiff.Value, node.Value - prev.Value);
            }

            State.Prev.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Prev = new();
        public static readonly AsyncLocal<int> MinDiff = new();
    }

    // A balanced BST over 0..size-1 - every adjacent in-order pair differs by
    // exactly 1, so the minimum difference is size-independent and never trivially
    // short-circuits after one comparison.
    private static BinaryTreeNode<int> BuildBalancedBst(int size)
    {
        var values = Enumerable.Range(0, size).ToArray();
        return BuildBalanced(values, 0, size - 1)!;
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
}
