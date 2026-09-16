using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndInorderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one strategy is
// ConstructBinaryTreeFromPreorderAndInorderTraversalSolution.BuildByPreorderIndexMap,
// the same method ...Tests proves correct. A balanced tree is built once via
// Fixtures.BinaryTrees and flattened into its own preorder/inorder arrays in
// [GlobalSetup], so only the reconstruction itself is measured.
[MemoryDiagnoser]
public class ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks
{
    private int[] _preorder = [];

    private int[] _inorder = [];
    [Params(2_000, 8_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = BinaryTrees.Balanced(NodeCount);
        _preorder = Preorder(tree);
        _inorder = Inorder(tree);
    }

    private static int[] Preorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        WalkPreorder(root, values);
        return [.. values];

        static void WalkPreorder(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null)
            {
                return;
            }
            values.Add(node.Value);
            WalkPreorder(node.Left, values);
            WalkPreorder(node.Right, values);
        }
    }

    private static int[] Inorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        WalkInorder(root, values);
        return [.. values];

        static void WalkInorder(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null)
            {
                return;
            }
            WalkInorder(node.Left, values);
            values.Add(node.Value);
            WalkInorder(node.Right, values);
        }
    }

    [Benchmark]
    public int PreorderIndexMap()
    {
        var tree = ConstructBinaryTreeFromPreorderAndInorderTraversalSolution.BuildByPreorderIndexMap(_preorder, _inorder);
        return CountNodes(tree);
    }

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : CountSubtree(node);

    // One for this node plus every node beneath it.
    private static int CountSubtree(BinaryTreeNode<int> node) =>
        1 + CountNodes(node.Left) + CountNodes(node.Right);
}
