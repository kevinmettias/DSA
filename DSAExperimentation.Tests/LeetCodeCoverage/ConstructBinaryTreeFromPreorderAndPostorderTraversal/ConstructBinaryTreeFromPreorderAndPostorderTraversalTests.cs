using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromPreorderAndPostorderTraversal;

// LeetCode 889. Construct Binary Tree from Preorder and Postorder Traversal: the
// same HashMap<TValue,TIndex>-indexed range-split shape
// ConstructBinaryTreeFromPreorderAndInorderTraversal/
// ConstructBinaryTreeFromInorderAndPostorderTraversal already use, just keyed off a
// postorder-position lookup instead of an inorder one. preorder[pre] right after a
// subtree's own root is that subtree's left child's root (when one exists), so
// looking its postorder index up locates the left/right split boundary directly.
public sealed partial class ConstructBinaryTreeFromPreorderAndPostorderTraversalTests
{
    [Fact]
    public void BuildTree_ClassicExample_ReconstructsBinaryTree()
    {
        var root = Build([1, 2, 4, 5, 3, 6, 7], [4, 5, 2, 6, 7, 3, 1]);

        Assert.Equal([1, 2, 4, 5, 3, 6, 7], PreOrder(root));
        Assert.Equal([4, 5, 2, 6, 7, 3, 1], PostOrder(root));
    }

    [Fact]
    public void BuildTree_SingleNode_ReturnsLeaf()
    {
        var root = Build([1], [1]);

        Assert.NotNull(root);
        Assert.Equal(1, root.Value);
        Assert.Null(root.Left);
        Assert.Null(root.Right);
    }

    private static BinaryTreeNode<int>? Build(int[] preorder, int[] postorder)
    {
        var postIndexOf = new HashMap<int, int>();
        for (var i = 0; i < postorder.Length; i++) postIndexOf.Set(postorder[i], i);

        var pre = 0;

        BinaryTreeNode<int>? BuildRange(int postLow, int postHigh)
        {
            if (postLow > postHigh) return null;

            var node = new BinaryTreeNode<int>(preorder[pre++]);
            if (postLow == postHigh) return node;

            postIndexOf.TryGetValue(preorder[pre], out var leftRootPostIndex);
            var leftSize = leftRootPostIndex - postLow + 1;

            node.Left = BuildRange(postLow, postLow + leftSize - 1);
            node.Right = BuildRange(postLow + leftSize, postHigh - 1);
            return node;
        }

        return BuildRange(0, postorder.Length - 1);
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
        => root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];

    private static int[] PostOrder(BinaryTreeNode<int>? root)
        => root is null ? [] : [.. PostOrder(root.Left), .. PostOrder(root.Right), root.Value];
}
