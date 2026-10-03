using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinaryTreeLevelOrderTraversalSolution's, the
// same methods BinaryTreeLevelOrderTraversalSolutionTests proves correct. The tree
// is complete - LeetCode's level-order array with no gaps, its values drawn from a
// seeded Random across LC 102's [-1000, 1000] - and NodeCount stops at LC 102's
// 2,000-node cap.
public class BinaryTreeLevelOrderTraversalBenchmarks
{
    private const int RandomSeed = 102; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    private BinaryTreeNode<int>? _root;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = BinaryTrees.Complete(levelOrder);
    }

    [Benchmark(Baseline = true)]
    public List<List<int>> QueueLevels() => BinaryTreeLevelOrderTraversalSolution.LevelOrderByQueueLevels(_root);

    [Benchmark]
    public List<List<int>> LevelGroupedTraversal() =>
        BinaryTreeLevelOrderTraversalSolution.LevelOrderByLevelGroupedTraversal(_root);
}
