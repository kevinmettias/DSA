using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeSolution's, the
// same methods PopulatingNextRightPointersInEachNodeSolutionTests proves correct. Fixture
// sizes are 2^k-1 so BinaryTrees.Balanced is a genuinely perfect tree, matching
// this problem's guarantee.
//
// Each arm returns .Count of its next-pointer map - a proxy, and deliberately one of the
// few left. The two strategies answer with different map types, the baseline a BCL
// Dictionary and the level-grouped arm the repo's HashMap, so returning the maps would
// compare two representations rather than two answers; rendering them alike would
// mean walking the tree inside the timed region. The fix belongs in the solution - one
// answer type for both strategies - and each strategy's own tests assert its map
// against LeetCode's examples meanwhile.
public class PopulatingNextRightPointersInEachNodeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(63, 1_023)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualQueueBfs() => PopulatingNextRightPointersInEachNodeSolution.ConnectByManualQueueBfs(_root).Count;

    [Benchmark]
    public int LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(_root).Count;
}
