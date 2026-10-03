using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalancedBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BalancedBinaryTreeSolution's, the same methods
// BalancedBinaryTreeTests proves correct. The original benchmark's two [Benchmark]
// arms (HeightCheck, BinaryTreeNodeCheck) called the exact same private helper - one
// real strategy, not two - so the pair here is that bottom-up single-pass recursion
// against the genuinely different top-down check, which re-measures each subtree's
// height once per ancestor. The built tree is balanced, so the top-down arm cannot
// bail out early and pays its full O(n log n) cost.
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
    public bool IsBalancedByHeightRecursion() => BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(_root);

    [Benchmark]
    public bool TopDownHeightCheck() => BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(_root);
}
