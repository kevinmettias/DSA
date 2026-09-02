using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeRightSideView;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinaryTreeRightSideViewSolution's, the same
// method BinaryTreeRightSideViewTests proves correct. A balanced tree keeps
// every level populated, so the buffered-level BFS does real width-proportional
// work at every depth instead of degenerating to a single-node-per-level walk.
[MemoryDiagnoser]
public class BinaryTreeRightSideViewBenchmarks
{
    [Params(255, 65_535)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark]
    public List<int> LevelGroupedTraversal() =>
        BinaryTreeRightSideViewSolution.RightSideViewByLevelGroupedTraversal(_root);
}
