using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeSolution's, the
// same methods PopulatingNextRightPointersInEachNodeSolutionTests proves correct. Fixture
// sizes are 2^k-1 so the complete tree BinaryTrees.Complete builds is a genuinely perfect
// tree, matching this problem's guarantee. Its level-order values wrap within LC 116's
// [-1000, 1000] rather than counting up to NodeCount: both strategies key their maps on
// node identity, so the values only have to stay in range. Each arm returns LeetCode's
// readout of the connected tree - every level along its next pointers, null for '#' -
// which both strategies produce through the same walk, so the arms differ only in how
// they link.
public class PopulatingNextRightPointersInEachNodeBenchmarks
{
    // The values 0..1000, every one of them inside LC 116's [-1000, 1000].
    private const int ValueSpan = 1_001;

    private BinaryTreeNode<int> _root = null!;

    [Params(63, 1_023)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Complete(LevelOrderValues(NodeCount));

    private static int[] LevelOrderValues(int nodeCount) =>
        [.. Enumerable.Range(0, nodeCount).Select(index => index % ValueSpan)];

    [Benchmark(Baseline = true)]
    public int?[] ManualQueueBfs() => PopulatingNextRightPointersInEachNodeSolution.ConnectByManualQueueBfs(_root);

    [Benchmark]
    public int?[] LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(_root);
}
