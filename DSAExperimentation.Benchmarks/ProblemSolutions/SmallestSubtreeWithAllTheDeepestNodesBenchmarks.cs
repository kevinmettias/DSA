using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestSubtreeWithAllTheDeepestNodesSolution's, the
// same methods SmallestSubtreeWithAllTheDeepestNodesTests proves correct - a
// hand-rolled (depth, node) recursion vs. this repo's own TreeFold engine closed
// over DeepestSubtreeAlgebra. The tree is built complete (heap-shaped) so recursion
// depth stays O(log n) at both sizes.
//
// Each arm reports the answer node's .Value rather than the node itself: a public
// [Benchmark] method cannot expose the internal BinaryTreeNode<int>, and the int is
// still enough to force the full (depth, node) computation through to a result.
[MemoryDiagnoser]
public class SmallestSubtreeWithAllTheDeepestNodesBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(2_000, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int HandRolledRecursion() =>
        SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByRecursion(_root)!.Value;

    [Benchmark]
    public int TreeFoldWithAlgebra() =>
        SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByTreeFold(_root)!.Value;
}
