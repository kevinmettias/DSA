using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeRightSideView;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinaryTreeRightSideViewSolution's, the same methods
// BinaryTreeRightSideViewSolutionTests proves correct. A balanced tree keeps every level
// populated, so the buffered-level BFS does real width-proportional work at every
// depth instead of degenerating to a single-node-per-level walk, while the
// right-first DFS follows one root-to-leaf path down and backfills the levels it
// skips.
public class BinaryTreeRightSideViewBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(255, 65_535)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public List<int> LevelGroupedTraversal() =>
        BinaryTreeRightSideViewSolution.RightSideViewByLevelGroupedTraversal(_root);

    [Benchmark]
    public List<int> DepthFirstRightFirst() =>
        BinaryTreeRightSideViewSolution.RightSideViewByDepthFirstRightFirst(_root);
}
