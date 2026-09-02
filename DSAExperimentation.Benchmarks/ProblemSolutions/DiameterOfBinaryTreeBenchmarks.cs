using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Diameter of Binary Tree (LC 543): a naive recursive height-per-node approach -
// for every node, recompute the height of its left and right subtrees from scratch
// via a fresh recursive walk, O(n) work times n nodes, O(n^2) overall - vs. this
// repo's own TreeMetrics.Diameter, whose DiameterAlgebra computes height and the
// best diameter-through-this-node together in a single bottom-up TreeFold pass,
// O(n).
[MemoryDiagnoser]
public class DiameterOfBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    // A left-skewed chain, not a bushy random tree: the naive baseline's O(n^2)
    // comes from recomputing a full-height walk at every node, and that only
    // shows up for real on an unbalanced tree - a random-parent tree stays
    // O(log n) deep, which lets the naive approach's real cost degrade to
    // O(n log n) and hide the split TreeMetricsDiameter's genuine O(n) is
    // actually being compared against.
    [GlobalSetup]
    public void Setup()
    {
        _root = new BinaryTreeNode<int>(0);
        var current = _root;

        for (var i = 1; i < NodeCount; i++)
        {
            current.Left = new BinaryTreeNode<int>(i);
            current = current.Left;
        }
    }

    [Benchmark(Baseline = true)]
    public int RecomputedHeightPerNode() => DiameterVia(_root).Diameter;

    private static (int Height, int Diameter) DiameterVia(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (0, 0);
        }

        var leftHeight = Height(node.Left);
        var rightHeight = Height(node.Right);
        var (_, leftDiameter) = DiameterVia(node.Left);
        var (_, rightDiameter) = DiameterVia(node.Right);

        var diameterThroughNode = leftHeight + rightHeight;
        var bestChildDiameter = Math.Max(leftDiameter, rightDiameter);
        var diameter = Math.Max(diameterThroughNode, bestChildDiameter);

        return (1 + Math.Max(leftHeight, rightHeight), diameter);
    }

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    [Benchmark]
    public int TreeMetricsDiameter()
        => TreeMetrics.Diameter<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(_root);
}
