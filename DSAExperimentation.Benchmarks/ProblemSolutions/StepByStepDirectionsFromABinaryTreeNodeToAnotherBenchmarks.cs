using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.StepByStepDirectionsFromABinaryTreeNodeToAnother;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution's, the same methods
// StepByStepDirectionsFromABinaryTreeNodeToAnotherTests proves correct. Both
// route between the leftmost and rightmost leaf of a balanced tree - an ancestor
// at the root, the worst case for either approach - and both are handed the
// prepared nodes their hoisted overload takes, so tree construction and endpoint
// selection are charged to [GlobalSetup] rather than to the search.
[MemoryDiagnoser]
public class StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    private BinaryTreeNode<int> _start = null!;
    private BinaryTreeNode<int> _dest = null!;
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Balanced(NodeCount);
        _start = LeftmostLeaf(_root);
        _dest = RightmostLeaf(_root);
    }

    private static BinaryTreeNode<int> LeftmostLeaf(BinaryTreeNode<int> node)
    {
        while (node.Left is not null || node.Right is not null)
        {
            node = node.Left ?? node.Right!;
        }

        return node;
    }

    private static BinaryTreeNode<int> RightmostLeaf(BinaryTreeNode<int> node)
    {
        while (node.Left is not null || node.Right is not null)
        {
            node = node.Right ?? node.Left!;
        }

        return node;
    }

    [Benchmark(Baseline = true)]
    public string DirectPathSearch() =>
        StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution.GetDirectionsByPathSearch(_root, _start, _dest);

    [Benchmark]
    public string LowestCommonAncestorWithRootToLeafPaths() =>
        StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution.GetDirectionsByLowestCommonAncestorPaths(
            _root, _start, _dest);
}
