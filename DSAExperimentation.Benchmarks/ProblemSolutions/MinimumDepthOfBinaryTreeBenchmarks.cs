using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumDepthOfBinaryTreeSolution's, the same methods
// MinimumDepthOfBinaryTreeSolutionTests proves correct. The pre-migration version carried
// two [Benchmark] methods (RecursiveMinDepth, BinaryTreeNodeMinDepth) that both
// called the exact same private recursive walk - one strategy measured twice, not
// two; the recursion is now measured against a level-order walk instead.
//
// The tree is complete - LeetCode's level-order array with no gaps, its values drawn
// from a seeded Random across LC 111's [-1000, 1000]. None of the sizes fills its
// last level, so the shallowest leaves sit one level above it: the level-order walk
// stops at the first of them without working through the deepest level, while the
// recursion visits every node. NodeCount stops at LC 111's 100,000-node cap.
public class MinimumDepthOfBinaryTreeBenchmarks
{
    private const int RandomSeed = 111; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    private BinaryTreeNode<int>? _root;

    [Params(1_000, 10_000, 100_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = BinaryTrees.Complete(levelOrder);
    }

    [Benchmark(Baseline = true)]
    public int RecursiveMinDepth() => MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(_root);

    [Benchmark]
    public int BreadthFirstSearch() => MinimumDepthOfBinaryTreeSolution.MinDepthByBreadthFirstSearch(_root);
}
