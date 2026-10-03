using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversalII;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinaryTreeLevelOrderTraversalIISolution's, the same
// methods BinaryTreeLevelOrderTraversalIISolutionTests proves correct. The tree is
// complete - LeetCode's level-order array with no gaps, its values drawn from a
// seeded Random across LC 107's [-1000, 1000] - and NodeCount stops at LC 107's
// 2,000-node cap.
public class BinaryTreeLevelOrderTraversalIIBenchmarks
{
    private const int RandomSeed = 107; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    private BinaryTreeNode<int>? _root;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = LeetCodeWireFormat.ToBinaryTree(Array.ConvertAll(levelOrder, value => (int?)value));
    }

    [Benchmark(Baseline = true)]
    public List<List<int>> QueueThenReverse() =>
        BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByQueue(_root);

    [Benchmark]
    public List<List<int>> LevelGroupedThenReverse() =>
        BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByLevelGroupedTraversal(_root);
}
