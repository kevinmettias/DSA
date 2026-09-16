using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is ValidateBinarySearchTreeSolution's, the same
// method ValidateBinarySearchTreeTests proves correct. The previous class
// carried RecursiveBounds and BinaryTreeNodeBounds as two [Benchmark] arms
// that both called the same private IsWithinBounds helper - one strategy under two
// names, not two - so only the survivor remains.
[MemoryDiagnoser]
public class ValidateBinarySearchTreeBenchmarks
{
    private const int RootValue = 2;
    private const int RightValue = 3;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = new BinaryTreeNode<int>(RootValue) { Left = new(1), Right = new(RightValue) };

    [Benchmark]
    public bool IsValidByBoundsRecursion() => ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(_root);
}
