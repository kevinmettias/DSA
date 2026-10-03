using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalancedBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BalancedBinaryTreeSolution's, the same methods
// BalancedBinaryTreeSolutionTests proves correct. The original benchmark's two [Benchmark]
// arms (HeightCheck, BinaryTreeNodeCheck) called the exact same private helper - one
// real strategy, not two - so the pair here is that bottom-up single-pass recursion
// against the genuinely different top-down check, which re-measures each subtree's
// height once per ancestor.
//
// The tree is complete - LeetCode's level-order array with no gaps, its values drawn
// from a seeded Random across LC 110's [-10^4, 10^4] - so it is balanced, and the
// top-down arm cannot bail out early and pays its full O(n log n) cost. NodeCount
// stops at LC 110's 5,000-node cap.
public class BalancedBinaryTreeBenchmarks
{
    private const int RandomSeed = 110; // LC problem number
    private const int MinValue = -10_000;
    private const int MaxValue = 10_000;

    private BinaryTreeNode<int>? _root;

    [Params(50, 500, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = BinaryTrees.Complete(levelOrder);
    }

    [Benchmark(Baseline = true)]
    public bool IsBalancedByHeightRecursion() => BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(_root);

    [Benchmark]
    public bool TopDownHeightCheck() => BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(_root);
}
