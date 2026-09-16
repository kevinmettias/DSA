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
    private int[] _inorder = [];

    private int[] _postorder = [];
    [Params(2_000, 8_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = BinaryTrees.Balanced(NodeCount);
        _inorder = Inorder(tree);
        _postorder = Postorder(tree);
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

    private static int[] Postorder(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        WalkPostorder(root, values);
        return [.. values];

        static void WalkPostorder(BinaryTreeNode<int>? node, List<int> values)
        {
            if (node is null)
            {
                return;
            }
            WalkPostorder(node.Left, values);
            WalkPostorder(node.Right, values);
            values.Add(node.Value);
        }
    }

    [Benchmark]
    public object? PostorderIndexMap() =>
        ConstructBinaryTreeFromInorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(_inorder, _postorder);
}
