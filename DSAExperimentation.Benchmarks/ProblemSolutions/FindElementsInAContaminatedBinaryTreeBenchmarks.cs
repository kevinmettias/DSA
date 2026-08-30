using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Elements in a Contaminated Binary Tree (LC 1261): recover values with a
// plain recursive DFS into a List<int>, then answer each Find with a linear
// Contains scan, vs. this repo's own TopDownTraversal to recover values (root.val =
// 0, Descend computes 2*parent+1/2*parent+2) into this repo's own Set<int>, giving
// O(1) Find. The tree is built as a complete binary tree, so node i's recovered
// value is exactly i - the same array-index-as-value shape a binary heap has.
[MemoryDiagnoser]
public class FindElementsInAContaminatedBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private int[] _targets = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = BuildCompleteTree(NodeCount);

        var random = new Random(1);
        _targets = Enumerable.Range(0, 200).Select(_ => random.Next(0, NodeCount * 2)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListRecoverThenLinearScan()
    {
        var values = new List<int>();
        Recover(_root, 0, values);

        var found = 0;

        foreach (var target in _targets)
        {
            if (values.Contains(target))
            {
                found++;
            }
        }

        return found;
    }

    [Benchmark]
    public int TopDownRecoverThenSetLookup()
    {
        var values = new Set<int>();

        TopDownTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            RecoverHooks, (int Value, Set<int> Found)>(_root, (0, values));

        var found = 0;

        foreach (var target in _targets)
        {
            if (values.Has(target))
            {
                found++;
            }
        }

        return found;
    }

    private static void Recover(BinaryTreeNode<int>? node, int value, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        node.Value = value;
        values.Add(value);
        Recover(node.Left, 2 * value + 1, values);
        Recover(node.Right, 2 * value + 2, values);
    }

    private static BinaryTreeNode<int> BuildCompleteTree(int nodeCount)
    {
        var nodes = new BinaryTreeNode<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(-1);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            var left = 2 * i + 1;
            var right = 2 * i + 2;

            if (left < nodeCount)
            {
                nodes[i].Left = nodes[left];
            }

            if (right < nodeCount)
            {
                nodes[i].Right = nodes[right];
            }
        }

        return nodes[0];
    }

    private readonly struct RecoverHooks : ITopDownHooks<BinaryTreeNode<int>, (int Value, Set<int> Found)>
    {
        public static void Visit(
            BinaryTreeNode<int> node, (int Value, Set<int> Found) state, int depth, NodePosition position)
        {
            node.Value = state.Value;
            state.Found.TryAdd(state.Value);
        }

        public static (int Value, Set<int> Found) Descend(
            BinaryTreeNode<int> parent, (int Value, Set<int> Found) parentState, BinaryTreeNode<int> child)
            => (child == parent.Left ? 2 * parentState.Value + 1 : 2 * parentState.Value + 2, parentState.Found);
    }
}
