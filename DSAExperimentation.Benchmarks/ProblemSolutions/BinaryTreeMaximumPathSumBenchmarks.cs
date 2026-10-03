using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeMaximumPathSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinaryTreeMaximumPathSumSolution's, the same
// method BinaryTreeMaximumPathSumSolutionTests proves correct. Pre-migration this class
// was an untested compile-smoke placeholder (`Baseline() => 1`,
// `PrimitiveComposed() => 1`) rather than a second strategy to reconcile.
//
// The tree is complete - LeetCode's level-order array with no gaps - with values
// drawn from a seeded Random across LC 124's whole [-1000, 1000], so a subtree's
// gain is often negative and dropped, and the running best is decided at nodes
// throughout the tree rather than only by the root's return value. NodeCount stops
// at LC 124's 30,000-node cap.
public class BinaryTreeMaximumPathSumBenchmarks
{
    private const int RandomSeed = 124; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    private BinaryTreeNode<int>? _root;

    [Params(300, 3_000, 30_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = BinaryTrees.Complete(levelOrder);
    }

    [Benchmark(Baseline = true)]
    public int GainRecursion() => BinaryTreeMaximumPathSumSolution.MaxPathSumByGainRecursion(_root);
}
