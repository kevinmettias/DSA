using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Construct Binary Search Tree from Preorder Traversal (LC 1008): this repo's own
// BinarySearchTree<int>.Insert, called once per value (a real compare-and-descend
// walk from the root every time - O(n) per insert on an unbalanced/ascending
// preorder, O(n^2) total) vs. the classic O(n) upper-bound recursion that reads
// preorder once and builds BinaryTreeNode<int> directly with no repeated
// root-to-leaf walk. Both produce this repo's own BinaryTreeNode<int> as the
// output Representation - the difference is purely which Operations approach
// gets there.
[MemoryDiagnoser]
public class ConstructBinarySearchTreeFromPreorderTraversalBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _ascendingPreorder = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Strictly ascending is itself a valid BST preorder (a fully right-skewed
        // tree) - the adversarial input that makes every BinarySearchTree.Insert
        // walk the full height built so far instead of O(log n) on average.
        _ascendingPreorder = Enumerable.Range(0, Length).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedTreeInsert()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _ascendingPreorder)
        {
            tree.Insert(value);
        }

        return tree.Count;
    }

    [Benchmark]
    public int BoundedRecursion() => CountNodes(BuildBounded(_ascendingPreorder));

    private static BinaryTreeNode<int>? BuildBounded(int[] preorder)
    {
        var index = 0;

        BinaryTreeNode<int>? Build(int bound)
        {
            if (index == preorder.Length || preorder[index] >= bound)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(preorder[index++]);
            node.Left = Build(node.Value);
            node.Right = Build(bound);
            return node;
        }

        return Build(int.MaxValue);
    }

    private static int CountNodes(BinaryTreeNode<int>? root) =>
        root is null ? 0 : 1 + CountNodes(root.Left) + CountNodes(root.Right);
}
