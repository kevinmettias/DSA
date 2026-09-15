using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the pre-migration version carried two [Benchmark] methods
// (RecursiveCompare, BinaryTreeNodeCompare) that both called the exact same private
// recursive comparison - one strategy measured twice, not two. SameTreeSolution has
// only IsSameByRecursiveCompare, so only one arm remains.
[MemoryDiagnoser]
public class SameTreeBenchmarks
{
    private const int LeftChildValue = 2;
    private const int RightChildValue = 3;

    private BinaryTreeNode<int> _a = null!;
    private BinaryTreeNode<int> _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        _a = Tree();
        _b = Tree();
    }

    private static BinaryTreeNode<int> Tree() => new(1) { Left = new(LeftChildValue), Right = new(RightChildValue) };

    [Benchmark]
    public bool RecursiveCompare() => SameTreeSolution.IsSameByRecursiveCompare(_a, _b);
}
