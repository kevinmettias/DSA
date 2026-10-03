using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeIISolution's, the
// same methods PopulatingNextRightPointersInEachNodeIISolutionTests proves correct.
// BinaryTrees.Skewed (a right-only chain, one node per level) is the extreme
// non-perfect shape - proof that neither strategy needs perfect-tree-specific code
// to stay correct here.
//
// Each arm returns .Count of its next-pointer map, a proxy kept for the reason
// PopulatingNextRightPointersInEachNodeBenchmarks gives: the strategies answer with a BCL
// Dictionary and a repo HashMap, two representations no rendering compares without
// walking the tree in the timed region.
public class PopulatingNextRightPointersInEachNodeIIBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualQueueBfs() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(_root).Count;

    [Benchmark]
    public int LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(_root).Count;
}
