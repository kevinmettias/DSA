using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class MaximumDepthOfBinaryTreeBenchmarks
{
    private const int RootValue = 3;
    private const int LeftChildValue = 9;
    private const int RightChildValue = 20;
    private const int RightLeftChildValue = 15;
    private const int RightRightChildValue = 7;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() =>
        _root = new(RootValue)
        {
            Left = new(LeftChildValue),
            Right = new(RightChildValue) { Left = new(RightLeftChildValue), Right = new(RightRightChildValue) },
        };

    [Benchmark(Baseline = true)]
    public int RecursiveHeight() => Height(_root);

    [Benchmark]
    public int TreeMetricsHeight() =>
        TreeMetrics.Height<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(_root);

    private static int Height(BinaryTreeNode<int>? n) => n is null ? 0 : 1 + Math.Max(Height(n.Left), Height(n.Right));
}
