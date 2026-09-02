using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Good Nodes in Binary Tree (LC 1448): a plain recursive DFS threading "max
// value on the root-to-node path" through call-stack parameters vs. this repo's own
// TopDownTraversal (FindElementsInAContaminatedBinaryTreeBenchmarks precedent,
// LC 1261) threading the same inherited state through ITopDownHooks.Descend and
// bumping a shared counter in Visit whenever a node's value is >= that max.
[MemoryDiagnoser]
public class CountGoodNodesInBinaryTreeBenchmarks
{
    private const int RandomSeed = 1448; // LC problem number
    private const int CompleteTreeBranchingFactor = 2;
    private const int RightChildIndexOffset = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _root = BuildCompleteTree(NodeCount, random);
    }

    [Benchmark(Baseline = true)]
    public int RecursiveDfs() => Count(_root, int.MinValue);

    [Benchmark]
    public int TopDownTraversalCount()
    {
        var counter = new Counter();

        TopDownTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            GoodNodeHooks, (int MaxSoFar, Counter Good)>(_root, (int.MinValue, counter));

        return counter.Count;
    }

    private static int Count(BinaryTreeNode<int>? node, int maxSoFar)
    {
        if (node is null)
        {
            return 0;
        }

        var good = node.Value >= maxSoFar ? 1 : 0;
        var nextMax = Math.Max(maxSoFar, node.Value);
        return good + Count(node.Left, nextMax) + Count(node.Right, nextMax);
    }

    private static BinaryTreeNode<int> BuildCompleteTree(int nodeCount, Random random)
    {
        var nodes = new BinaryTreeNode<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(random.Next(0, nodeCount));
        }

        LinkChildren(nodes, nodeCount);

        return nodes[0];
    }

    // Wires each index's array-implicit children per the standard complete-binary-tree
    // indexing formula (left = 2i+1, right = 2i+2).
    private static void LinkChildren(BinaryTreeNode<int>[] nodes, int nodeCount)
    {
        for (var i = 0; i < nodeCount; i++)
        {
            var left = (CompleteTreeBranchingFactor * i) + 1;
            var right = (CompleteTreeBranchingFactor * i) + RightChildIndexOffset;

            if (left < nodeCount)
            {
                nodes[i].Left = nodes[left];
            }

            if (right < nodeCount)
            {
                nodes[i].Right = nodes[right];
            }
        }
    }

    private sealed class Counter
    {
        public int Count;
    }

    private readonly struct GoodNodeHooks : ITopDownHooks<BinaryTreeNode<int>, (int MaxSoFar, Counter Good)>
    {
        public static void Visit(
            BinaryTreeNode<int> node, (int MaxSoFar, Counter Good) state, int depth, NodePosition position)
        {
            if (node.Value >= state.MaxSoFar)
            {
                state.Good.Count++;
            }
        }

        public static (int MaxSoFar, Counter Good) Descend(
            BinaryTreeNode<int> parent, (int MaxSoFar, Counter Good) parentState, BinaryTreeNode<int> child)
            => (Math.Max(parentState.MaxSoFar, parent.Value), parentState.Good);
    }
}
