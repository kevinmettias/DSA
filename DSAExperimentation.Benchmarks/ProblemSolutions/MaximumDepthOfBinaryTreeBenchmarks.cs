using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MaximumDepthOfBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumDepthOfBinaryTreeSolution's, the same
// methods MaximumDepthOfBinaryTreeSolutionTests proves correct. The tree is complete -
// LeetCode's level-order array with no gaps, its values drawn from a seeded Random
// across LC 104's [-100, 100] - and NodeCount stops at LC 104's 10,000-node cap.
public class MaximumDepthOfBinaryTreeBenchmarks
{
    private const int RandomSeed = 104; // LC problem number
    private const int MinValue = -100;
    private const int MaxValue = 100;

    private BinaryTreeNode<int>? _root;

    [Params(100, 1_000, 10_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = LeetCodeWireFormat.ToBinaryTree(Array.ConvertAll(levelOrder, value => (int?)value));
    }

    [Benchmark(Baseline = true)]
    public int RecursiveHeight() => MaximumDepthOfBinaryTreeSolution.MaxDepthByRecursion(_root);

    [Benchmark]
    public int TreeMetricsHeight() => MaximumDepthOfBinaryTreeSolution.MaxDepthByTreeMetrics(_root);
}
