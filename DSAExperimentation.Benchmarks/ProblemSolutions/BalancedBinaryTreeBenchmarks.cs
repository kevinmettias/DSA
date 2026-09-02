using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalancedBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BalancedBinaryTreeSolution's, the same
// method BalancedBinaryTreeTests proves correct. The original benchmark's
// two [Benchmark] arms (HeightCheck, BinaryTreeNodeCheck) called the exact
// same private helper - one real strategy, not two - so there is only one
// arm here too, mirroring ConvertSortedArrayToBinarySearchTreeBenchmarks'
// precedent for a single-strategy problem.
[MemoryDiagnoser]
public class BalancedBinaryTreeBenchmarks
{
    private const int RootValue = 3;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() =>
        _root = new(RootValue)
        {
            Left = new(LeftValue),
            Right = new(RightValue) { Left = new(RightLeftValue), Right = new(RightRightValue) },
        };

    [Benchmark(Baseline = true)]
    public bool HeightRecursion() => BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(_root);
}
