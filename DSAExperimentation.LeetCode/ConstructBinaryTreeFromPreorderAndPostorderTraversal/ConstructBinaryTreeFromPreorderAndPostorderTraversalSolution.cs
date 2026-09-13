using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

// LeetCode 889. Construct Binary Tree from Preorder and Postorder Traversal:
// preorder's next unread value is the current subtree's root, and the value right
// after it - preorder[pre] - is that subtree's left child's root whenever one
// exists. Its position in postorder is the last index of the left subtree, so it
// locates the left/right split boundary directly.
//
// Both strategies walk that same split; they differ only in how the left root's
// postorder position is found - rescanning the postorder range linearly (O(n) per
// node, O(n^2) overall on a skewed input) or looking it up in a value->index map
// built once (O(1) per node, O(n) overall).
internal static class ConstructBinaryTreeFromPreorderAndPostorderTraversalSolution
{
    // The textbook answer: scan the live postorder range for the left child's root
    // at every node, no index built up front. Deliberately written without this
    // repo's primitives - it is the arm the indexed strategy below has to justify
    // itself against.
    public static BinaryTreeNode<int>? BuildByPostorderScan(int[] preorder, int[] postorder)
    {
        var walk = new ScanningWalk(preorder, postorder);

        return walk.BuildRange(0, postorder.Length - 1);
    }

    // This repo's own HashMap<TValue,TIndex> gives the same postorder lookup in
    // O(1), the same value->index shape
    // ConstructBinaryTreeFromPreorderAndInorderTraversal uses, keyed off postorder
    // position instead of inorder position.
    public static BinaryTreeNode<int>? BuildByPostorderIndexMap(int[] preorder, int[] postorder)
    {
        var postIndexOf = new HashMap<int, int>();

        for (var i = 0; i < postorder.Length; i++)
        {
            postIndexOf.Set(postorder[i], i);
        }

        var walk = new IndexedWalk(preorder, postIndexOf);

        return walk.BuildRange(0, postorder.Length - 1);
    }

    // Tracks how far into preorder each walk has consumed, so every subtree's root
    // is read in preorder's own order: this root, then its whole left subtree, then
    // its whole right one. The two walks differ only in FindLeftRootPostIndex.
    private struct ScanningWalk(int[] preorder, int[] postorder)
    {
        private readonly int[] _preorder = preorder;
        private readonly int[] _postorder = postorder;
        private int _next;

        public BinaryTreeNode<int>? BuildRange(int postLow, int postHigh)
        {
            if (postLow > postHigh)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(_preorder[_next++]);

            if (postLow == postHigh)
            {
                return node;
            }

            var leftSize = FindLeftRootPostIndex(postLow, postHigh) - postLow + 1;
            node.Left = BuildRange(postLow, postLow + leftSize - 1);
            node.Right = BuildRange(postLow + leftSize, postHigh - 1);
            return node;
        }

        private readonly int FindLeftRootPostIndex(int postLow, int postHigh)
        {
            var leftRootValue = _preorder[_next];

            for (var i = postLow; i <= postHigh; i++)
            {
                if (_postorder[i] == leftRootValue)
                {
                    return i;
                }
            }

            return postLow;
        }
    }

    private struct IndexedWalk(int[] preorder, HashMap<int, int> postIndexOf)
    {
        private readonly int[] _preorder = preorder;
        private readonly HashMap<int, int> _postIndexOf = postIndexOf;
        private int _next;

        public BinaryTreeNode<int>? BuildRange(int postLow, int postHigh)
        {
            if (postLow > postHigh)
            {
                return null;
            }

            var node = new BinaryTreeNode<int>(_preorder[_next++]);

            if (postLow == postHigh)
            {
                return node;
            }

            _postIndexOf.TryGetValue(_preorder[_next], out var leftRootPostIndex);
            var leftSize = leftRootPostIndex - postLow + 1;

            node.Left = BuildRange(postLow, postLow + leftSize - 1);
            node.Right = BuildRange(postLow + leftSize, postHigh - 1);
            return node;
        }
    }
}
