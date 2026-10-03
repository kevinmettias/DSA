using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestSubtreeWithAllTheDeepestNodesSolution's, the
// same methods SmallestSubtreeWithAllTheDeepestNodesSolutionTests proves correct - a
// hand-rolled (depth, node) recursion vs. this repo's own TreeFold engine closed
// over DeepestSubtreeAlgebra. The tree is built complete (heap-shaped) so recursion
// depth stays O(log n) at both sizes.
//
// Each arm returns the answer node itself, as object because a public [Benchmark]
// method cannot expose the internal BinaryTreeNode<int>.
public class SmallestSubtreeWithAllTheDeepestNodesBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(2_000, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public object? HandRolledRecursion() =>
        SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByRecursion(_root);

    [Benchmark]
    public object? TreeFoldWithAlgebra() =>
        SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByTreeFold(_root);
}
