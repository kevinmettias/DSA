using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeIISolution's, the
// same methods PopulatingNextRightPointersInEachNodeIISolutionTests proves correct.
// A right-only chain, one node per level, is the extreme non-perfect shape - proof
// that neither strategy needs perfect-tree-specific code to stay correct here. It is
// built here rather than by BinaryTrees.Skewed, whose values count up to NodeCount:
// these wrap within LC 117's [-100, 100], and since both strategies key their maps on
// node identity, the values only have to stay in range. Each arm returns LeetCode's
// readout of the connected tree - here one value and a '#' (null) per level.
public class PopulatingNextRightPointersInEachNodeIIBenchmarks
{
    // The values 0..100, every one of them inside LC 117's [-100, 100].
    private const int ValueSpan = 101;

    private BinaryTreeNode<int> _root = null!;

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = RightChain(NodeCount);

    private static BinaryTreeNode<int> RightChain(int nodeCount)
    {
        var root = new BinaryTreeNode<int>(0);
        var tail = root;

        for (var index = 1; index < nodeCount; index++)
        {
            tail.Right = new BinaryTreeNode<int>(index % ValueSpan);
            tail = tail.Right;
        }

        return root;
    }

    [Benchmark(Baseline = true)]
    public int?[] ManualQueueBfs() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(_root);

    [Benchmark]
    public int?[] LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(_root);
}
