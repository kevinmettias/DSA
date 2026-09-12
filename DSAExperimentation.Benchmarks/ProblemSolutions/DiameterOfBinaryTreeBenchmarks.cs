using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DiameterOfBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Diameter of Binary Tree (LC 543): a naive recursive height-per-node approach,
// O(n^2) overall, vs. this repo's own TreeMetrics.Diameter fold, O(n). See
// DiameterOfBinaryTreeSolution for what each strategy does.
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
    // O(n log n) and hide the split TreeMetrics.Diameter's genuine O(n) is
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
    public int RecomputedHeightPerNode() => DiameterOfBinaryTreeSolution.DiameterByRecomputedHeightPerNode(_root);

    [Benchmark]
    public int TreeMetricsDiameter() => DiameterOfBinaryTreeSolution.DiameterByTreeMetricsFold(_root);
}
