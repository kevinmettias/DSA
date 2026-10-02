using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumDepthOfBinaryTreeSolution's, the same methods
// MinimumDepthOfBinaryTreeTests proves correct. The pre-migration version carried
// two [Benchmark] methods (RecursiveMinDepth, BinaryTreeNodeMinDepth) that both
// called the exact same private recursive walk - one strategy measured twice, not
// two; the recursion is now measured against a level-order walk instead. The tree
// is unchanged.
[MemoryDiagnoser]
public class MinimumDepthOfBinaryTreeBenchmarks
{
    private const int LeftChildValue = 2;
    private const int RightChildValue = 3;
    private const int RightGrandchildValue = 4;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = new(1) { Left = new(LeftChildValue), Right = new(RightChildValue) { Right = new(RightGrandchildValue) } };

    [Benchmark(Baseline = true)]
    public int RecursiveMinDepth() => MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(_root);

    [Benchmark]
    public int BreadthFirstSearch() => MinimumDepthOfBinaryTreeSolution.MinDepthByBreadthFirstSearch(_root);
}
