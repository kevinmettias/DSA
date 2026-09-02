using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructBinaryTreeFromInorderAndPostorderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one strategy is
// ConstructBinaryTreeFromInorderAndPostorderTraversalSolution.BuildByPostorderIndexMap,
// the same method ...Tests proves correct. A balanced tree is built once via
// Fixtures.BinaryTrees and flattened into its own inorder/postorder arrays in
// [GlobalSetup], so only the reconstruction itself is measured.
[MemoryDiagnoser]
public class ConstructBinaryTreeFromInorderAndPostorderTraversalBenchmarks
{
    [Params(2_000, 8_000)]
    public int NodeCount;

    private int[] _inorder = null!;
    private int[] _postorder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tree = BinaryTrees.Balanced(NodeCount);
        _inorder = Inorder(tree);
        _postorder = Postorder(tree);
    }

    [Benchmark]
    public object? PostorderIndexMap() =>
        ConstructBinaryTreeFromInorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(_inorder, _postorder);

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

    private static int[] Postorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        Walk(root, values);
        return [.. values];

        static void Walk(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null) return;
            Walk(node.Left, values);
            Walk(node.Right, values);
            values.Add(node.Value);
        }
    }
}
