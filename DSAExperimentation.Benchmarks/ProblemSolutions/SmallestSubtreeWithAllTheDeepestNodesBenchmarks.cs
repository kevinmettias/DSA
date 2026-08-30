using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest Subtree with all the Deepest Nodes (LC 865): a hand-rolled recursive
// (depth, node) pass directly over BinaryTreeNode<int>.Left/Right vs. this repo's
// own generic TreeFold engine closed over a locally-defined DeepestSubtreeAlgebra -
// the same "hand-rolled vs. generic repo engine, same O(n) complexity class,
// difference is composability/dispatch overhead" framing WaterAndJugProblem
// Benchmarks already establishes for DepthFirstSearch.Traverse. The tree is built
// complete (heap-shaped) so recursion depth stays O(log n) at both sizes.
[MemoryDiagnoser]
public class SmallestSubtreeWithAllTheDeepestNodesBenchmarks
{
    [Params(2_000, 20_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BuildCompleteTree(NodeCount);

    // Returns .Value (int), not the internal BinaryTreeNode<int> itself - a public
    // [Benchmark] method can't expose an internal return type, and the int is still
    // enough to force the full (depth, node) computation through to a result.
    [Benchmark(Baseline = true)]
    public int HandRolledRecursion() => Deepest(_root).Node!.Value;

    [Benchmark]
    public int TreeFoldWithAlgebra()
        => TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            DeepestSubtreeAlgebra, (int Depth, BinaryTreeNode<int>? Node)>(_root).Node!.Value;

    private static BinaryTreeNode<int> BuildCompleteTree(int count)
    {
        var nodes = new BinaryTreeNode<int>[count];
        for (var i = 0; i < count; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(i);
        }

        for (var i = 0; i < count; i++)
        {
            var leftIndex = 2 * i + 1;
            var rightIndex = 2 * i + 2;
            if (leftIndex < count)
            {
                nodes[i].Left = nodes[leftIndex];
            }

            if (rightIndex < count)
            {
                nodes[i].Right = nodes[rightIndex];
            }
        }

        return nodes[0];
    }

    private static (int Depth, BinaryTreeNode<int>? Node) Deepest(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (-1, null);
        }

        var left = Deepest(node.Left);
        var right = Deepest(node.Right);

        if (left.Depth > right.Depth)
        {
            return (left.Depth + 1, left.Node);
        }

        if (right.Depth > left.Depth)
        {
            return (right.Depth + 1, right.Node);
        }

        return (left.Depth + 1, node);
    }

    private readonly struct DeepestSubtreeAlgebra
        : IFoldAlgebra<BinaryTreeNode<int>, (int Depth, BinaryTreeNode<int>? Node)>
    {
        public static (int Depth, BinaryTreeNode<int>? Node) Empty => (-1, null);

        public static (int Depth, BinaryTreeNode<int>? Node) Combine(
            BinaryTreeNode<int> node, IReadOnlyList<(int Depth, BinaryTreeNode<int>? Node)> children)
        {
            if (children.Count == 0)
            {
                return (0, node);
            }

            var maxDepth = 0;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].Depth > maxDepth)
                {
                    maxDepth = children[i].Depth;
                }
            }

            BinaryTreeNode<int>? deepest = null;
            var tieCount = 0;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].Depth == maxDepth)
                {
                    tieCount++;
                    deepest = children[i].Node;
                }
            }

            return (maxDepth + 1, tieCount == 1 ? deepest : node);
        }
    }
}
