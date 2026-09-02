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
    [Params(2_000, 8_000)]
    public int NodeCount;

    private int[] _preorder = null!;
    private int[] _inorder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tree = BinaryTrees.Balanced(NodeCount);
        _preorder = Preorder(tree);
        _inorder = Inorder(tree);
    }

    [Benchmark]
    public int PreorderIndexMap() =>
        CountNodes(ConstructBinaryTreeFromPreorderAndInorderTraversalSolution.BuildByPreorderIndexMap(_preorder, _inorder));

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static int[] Preorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        Walk(root, values);
        return [.. values];

        static void Walk(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null) return;
            values.Add(node.Value);
            Walk(node.Left, values);
            Walk(node.Right, values);
        }
    }

    private static int[] Inorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        Walk(root, values);
        return [.. values];

        static void Walk(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null) return;
            Walk(node.Left, values);
            values.Add(node.Value);
            Walk(node.Right, values);
        }
    }
}
