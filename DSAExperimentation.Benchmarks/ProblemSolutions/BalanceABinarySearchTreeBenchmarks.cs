using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Balance a Binary Search Tree (LC 1382): a naive way to collect an already-BST's
// values in sorted order without trusting its shape is to re-derive the k-th
// smallest value from scratch for every rank k = 1..n - each call is its own O(n)
// in-order walk, O(n^2) overall - vs. this repo's own InOrderTraversal/
// IInOrderHooks, which visits every node exactly once (O(n)) to collect the same
// sorted values into a DynamicArray<int>, the same composition FindModeInBinaryS
// earchTreeBenchmarks/KthSmallestElementInABSTBenchmarks already use. Both then
// rebuild via the identical midpoint-split recursion ConvertSortedArrayToBinaryS
// earchTreeBenchmarks already proves is sufficient on its own. Fixtures.BinaryTre
// es.Skewed gives a maximally unbalanced (but already-BST-ordered) input tree.
[MemoryDiagnoser]
public class BalanceABinarySearchTreeBenchmarks
{
    private const int MidpointDivisor = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int RepeatedKthSmallestScan()
    {
        var sorted = new int[NodeCount];

        for (var k = 1; k <= NodeCount; k++)
        {
            sorted[k - 1] = FindKthSmallest(_root, k);
        }

        var rebuilt = Build(sorted, 0, sorted.Length - 1);
        return Height(rebuilt);
    }

    [Benchmark]
    public int InOrderTraversalCollectAndRebuild()
    {
        State.Sorted.Value = new DynamicArray<int>();

        InOrderTraversal.Walk<int, CollectHooks>(_root);

        var sorted = State.Sorted.Value;
        var rebuilt = BuildFromDynamicArray(sorted, 0, sorted.Count - 1);
        return Height(rebuilt);
    }

    private static int FindKthSmallest(BinaryTreeNode<int> root, int k)
    {
        var remaining = k;
        var result = 0;
        Visit(root);
        return result;

        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null || remaining == 0)
            {
                return;
            }

            Visit(node.Left);

            if (remaining == 0)
            {
                return;
            }

            remaining--;

            if (remaining == 0)
            {
                result = node.Value;
                return;
            }

            Visit(node.Right);
        }
    }

    private static BinaryTreeNode<int>? Build(int[] sorted, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / MidpointDivisor);
        return new BinaryTreeNode<int>(sorted[mid])
        {
            Left = Build(sorted, low, mid - 1),
            Right = Build(sorted, mid + 1, high),
        };
    }

    private static BinaryTreeNode<int>? BuildFromDynamicArray(DynamicArray<int> sorted, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / MidpointDivisor);
        return new BinaryTreeNode<int>(sorted.Get(mid))
        {
            Left = BuildFromDynamicArray(sorted, low, mid - 1),
            Right = BuildFromDynamicArray(sorted, mid + 1, high),
        };
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Sorted.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Sorted = new();
    }
}
