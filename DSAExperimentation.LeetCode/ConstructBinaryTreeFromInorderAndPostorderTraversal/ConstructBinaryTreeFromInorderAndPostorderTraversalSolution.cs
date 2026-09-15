using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ConstructBinaryTreeFromInorderAndPostorderTraversal;

// LeetCode 106. Construct Binary Tree from Inorder and Postorder Traversal:
// postorder's next unread value, taken from the end, is always the current
// subtree's root, and inorder's position for that value splits the rest of the
// subtree into its left and right ranges - the mirror image of LC 105's
// left-to-right preorder walk. A value->index map over inorder turns that split
// into an O(1) lookup instead of a linear scan, so the whole tree is rebuilt in
// O(n).
internal static class ConstructBinaryTreeFromInorderAndPostorderTraversalSolution
{
    public static BinaryTreeNode<int>? BuildByPostorderIndexMap(int[] inorder, int[] postorder)
    {
        var inorderIndex = new HashMap<int, int>();

        for (var i = 0; i < inorder.Length; i++)
        {
            inorderIndex.Set(inorder[i], i);
        }

        var walk = new PostorderWalk(postorder, inorderIndex);

        return walk.BuildRange(0, inorder.Length - 1);
    }

    // Tracks how far into postorder the walk has consumed, read back-to-front so
    // each subtree's root is read in postorder's own reverse order: this subtree's
    // root, then its whole right subtree, then its whole left one.
    private sealed class PostorderWalk(int[] postorder, HashMap<int, int> inorderIndex)
    {
        private readonly int[] _postorder = postorder;
        private readonly HashMap<int, int> _inorderIndex = inorderIndex;
        private int _next = postorder.Length - 1;

        public BinaryTreeNode<int>? BuildRange(int low, int high)
        {
            if (low > high)
            {
                return null;
            }

            var value = _postorder[_next--];
            _inorderIndex.TryGetValue(value, out var mid);

            return new BinaryTreeNode<int>(value)
            {
                Right = BuildRange(mid + 1, high),
                Left = BuildRange(low, mid - 1),
            };
        }
    }
}
