using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SearchInABinarySearchTree;

// LeetCode 700. Search in a Binary Search Tree: given a BST root and a value,
// return the subtree rooted at the node holding that value (its own children
// come along for free), or null if the value is absent.
internal static class SearchInABinarySearchTreeSolution
{
    // The naive baseline: visit every node with no BST ordering exploited - a
    // plain stack-driven walk, as if this were an arbitrary binary tree.
    // Deliberately BCL-only inside (a plain Stack<T>): the point is contrasting
    // against the ordering exploited below.
    public static BinaryTreeNode<int>? SearchBstByLinearScan(BinaryTreeNode<int>? root, int val)
    {
        var stack = new Stack<BinaryTreeNode<int>>();

        if (root is not null)
        {
            stack.Push(root);
        }

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.Value == val)
            {
                return node;
            }

            if (node.Left is not null)
            {
                stack.Push(node.Left);
            }

            if (node.Right is not null)
            {
                stack.Push(node.Right);
            }
        }

        return null;
    }

    // Exploit the BST invariant: compare against the current node and descend
    // left or right, discarding half the remaining subtree each step - the same
    // walk BinarySearchTree<TValue>.Has already performs internally.
    public static BinaryTreeNode<int>? SearchBstByBstDescent(BinaryTreeNode<int>? root, int val)
    {
        var node = root;

        while (node is not null && node.Value != val)
        {
            node = val < node.Value ? node.Left : node.Right;
        }

        return node;
    }
}
