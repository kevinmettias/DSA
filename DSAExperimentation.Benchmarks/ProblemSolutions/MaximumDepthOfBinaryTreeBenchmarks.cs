using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumDepthOfBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumDepthOfBinaryTreeSolution's, the same
// methods MaximumDepthOfBinaryTreeTests proves correct.
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
    public int RecursiveHeight() => MaximumDepthOfBinaryTreeSolution.MaxDepthByRecursion(_root);

    [Benchmark]
    public int TreeMetricsHeight() => MaximumDepthOfBinaryTreeSolution.MaxDepthByTreeMetrics(_root);
}
