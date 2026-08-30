using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinarySearchTreeFromPreorderTraversal;

// LeetCode 1008. Construct Binary Search Tree from Preorder Traversal: inserting
// preorder's values in order into this repo's own BinarySearchTree<int> rebuilds
// the exact source tree - a BST's preorder is root, then every value less than
// it (its left subtree's own preorder), then every value greater (its right
// subtree's own preorder), which is precisely the branch BinarySearchTree.Insert's
// compare-and-descend walk takes for each value in turn.
public sealed partial class ConstructBinarySearchTreeFromPreorderTraversalTests
{
    [Fact]
    public void BstFromPreorder_ClassicExample_PreorderOfRebuiltTreeMatchesInput()
    {
        var root = BstFromPreorder([8, 5, 1, 7, 10, 12]);

        Assert.Equal([8, 5, 1, 7, 10, 12], PreOrder(root));
    }

    [Fact]
    public void BstFromPreorder_SingleValue_ReturnsSingleNodeTree()
    {
        var root = BstFromPreorder([1]);

        Assert.Equal([1], PreOrder(root));
    }

    private static BinaryTreeNode<int>? BstFromPreorder(int[] preorder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in preorder)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root) =>
        root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
