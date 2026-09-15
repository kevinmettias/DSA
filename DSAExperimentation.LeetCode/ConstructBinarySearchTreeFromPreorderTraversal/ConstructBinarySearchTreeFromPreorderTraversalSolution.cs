using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ConstructBinarySearchTreeFromPreorderTraversal;

// LeetCode 1008. Construct Binary Search Tree from Preorder Traversal: rebuild the
// tree whose preorder is the given array of distinct values.
//
// Both strategies return this repo's own BinaryTreeNode<int> - the difference is
// purely which Operations approach gets there:
//
// - Inserting the values in order into BinarySearchTree<int> rebuilds the exact
//   source tree, because a BST's preorder is root, then every value less than it
//   (its left subtree's own preorder), then every value greater (its right
//   subtree's own preorder) - precisely the branch Insert's compare-and-descend
//   walk takes for each value in turn. It pays a fresh root-to-leaf walk per
//   value, so O(n) each and O(n^2) total on an unbalanced (e.g. ascending) input.
// - The classic upper-bound recursion reads preorder once, left to right, giving
//   each recursive call the exclusive upper bound its subtree may contain, and
//   builds the nodes directly - O(n) with no repeated descent.
internal static class ConstructBinarySearchTreeFromPreorderTraversalSolution
{
    // Compose this repo's BinarySearchTree and hand back the tree it built.
    public static BinaryTreeNode<int>? BstFromPreorderByRepeatedInsert(int[] preorder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in preorder)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }

    // One left-to-right pass. Values are distinct, so int.MaxValue is a safe
    // "no upper bound" sentinel for the root call.
    public static BinaryTreeNode<int>? BstFromPreorderByUpperBoundRecursion(int[] preorder) =>
        new BoundedWalk(preorder).Build(int.MaxValue);

    // The cursor into preorder is shared by every frame of the recursion, which is
    // what lets each subtree consume exactly the prefix that belongs to it - hence
    // a reference type rather than a value copied independently into each frame.
    private sealed class BoundedWalk(int[] preorder)
    {
        private readonly int[] _preorder = preorder;
        private int _index;

        public BinaryTreeNode<int>? Build(int bound)
        {
            if (_index == _preorder.Length || _preorder[_index] >= bound)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(_preorder[_index++]);
            node.Left = Build(node.Value);
            node.Right = Build(bound);
            return node;
        }
    }
}
