using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndInorderTraversal;

// LeetCode 105. Construct Binary Tree from Preorder and Inorder Traversal: preorder's
// next unread value is always the current subtree's root, and inorder's position for
// that value splits the rest of the subtree into its left and right ranges. A
// value->index map over inorder turns that split into an O(1) lookup instead of a
// linear scan, so the whole tree is rebuilt in O(n).
internal static class ConstructBinaryTreeFromPreorderAndInorderTraversalSolution
{
    public static BinaryTreeNode<int>? BuildByPreorderIndexMap(int[] preorder, int[] inorder)
    {
        var inorderIndex = new HashMap<int, int>();

        for (var i = 0; i < inorder.Length; i++)
        {
            inorderIndex.Set(inorder[i], i);
        }

        var walk = new PreorderWalk(preorder, inorderIndex);

        return walk.BuildRange(0, inorder.Length - 1);
    }

    // Tracks how far into preorder the walk has consumed, shared across every
    // recursive call so the root of each subtree is read in preorder's own order:
    // this subtree's root, then its whole left subtree, then its whole right one.
    private sealed class PreorderWalk(int[] preorder, HashMap<int, int> inorderIndex)
    {
        private readonly int[] _preorder = preorder;
        private readonly HashMap<int, int> _inorderIndex = inorderIndex;
        private int _next;

        public BinaryTreeNode<int>? BuildRange(int low, int high)
        {
            if (low > high)
            {
                return null;
            }

            var value = _preorder[_next++];
            _inorderIndex.TryGetValue(value, out var mid);

            return new BinaryTreeNode<int>(value)
            {
                Left = BuildRange(low, mid - 1),
                Right = BuildRange(mid + 1, high),
            };
        }
    }
}
