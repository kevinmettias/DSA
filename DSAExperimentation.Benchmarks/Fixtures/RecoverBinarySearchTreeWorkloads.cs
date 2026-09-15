using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 99 - a balanced BST over 0..size-1 with its min
// and max values swapped, producing exactly one non-adjacent violation pair for the
// recovery strategies to find and fix, independent of size.
internal static class RecoverBinarySearchTreeWorkloads
{

    public static BinaryTreeNode<int> BuildCorruptedBst(int size)
    {
        var root = BuildBalancedTree(size);
        SwapMinAndMaxValues(root);

        return root;
    }

    // A balanced tree over 0..size-1, the ordered shape both violations are read off.
    private static BinaryTreeNode<int> BuildBalancedTree(int size)
    {
        var values = Enumerable.Range(0, size).ToArray();

        return BuildBalanced(values, 0, size - 1)!;
    }

    // Swaps the tree's two extremal values, which is exactly the one non-adjacent
    // violation pair the recovery strategies have to find and fix.
    private static void SwapMinAndMaxValues(BinaryTreeNode<int> root)
    {
        var min = FindMin(root);
        var max = FindMax(root);
        (min.Value, max.Value) = (max.Value, min.Value);
    }

    private static BinaryTreeNode<int> FindMin(BinaryTreeNode<int> node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }

    private static BinaryTreeNode<int> FindMax(BinaryTreeNode<int> node)
    {
        while (node.Right is not null)
        {
            node = node.Right;
        }

        return node;
    }

    private static BinaryTreeNode<int>? BuildBalanced(int[] values, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

        return new BinaryTreeNode<int>(values[mid])
        {
            Left = BuildBalanced(values, low, mid - 1),
            Right = BuildBalanced(values, mid + 1, high),
        };
    }
}
