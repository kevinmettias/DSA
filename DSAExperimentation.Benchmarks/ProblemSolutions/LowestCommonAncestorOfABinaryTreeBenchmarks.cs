using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfABinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the arm is LowestCommonAncestorOfABinaryTreeSolution's, the same
// method LowestCommonAncestorOfABinaryTreeTests proves correct. Queries the
// leftmost and rightmost leaf of a balanced tree - an LCA at the root, the worst
// case for the search.
//
// Returns object, not BinaryTreeNode<int> - the node type is internal, so a public
// [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class LowestCommonAncestorOfABinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private BinaryTreeNode<int> _p = null!;
    private BinaryTreeNode<int> _q = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Balanced(NodeCount);
        _p = LeftmostLeaf(_root);
        _q = RightmostLeaf(_root);
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

    [Benchmark]
    public object? AncestryWalk() =>
        LowestCommonAncestorOfABinaryTreeSolution.FindLcaByAncestryWalk(_root, _p, _q);
}
