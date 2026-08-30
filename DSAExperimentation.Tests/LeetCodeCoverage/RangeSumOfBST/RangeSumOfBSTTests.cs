using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumOfBST;

// LeetCode 938. Range Sum of BST: a recursive walk directly over BinaryTreeNode<int>
// that prunes using the BST ordering invariant - the exact "return Trim(node.Right/
// Left, ...)" shape TrimABinarySearchTreeTests already uses for the same problem
// family, just summing survivors instead of re-parenting them. A node below low
// only has anything relevant in its right subtree; a node above high only in its
// left - so an entire out-of-range subtree is skipped without ever visiting it,
// unlike a plain full-tree traversal.
public sealed class RangeSumOfBSTTests
{
    [Fact]
    public void RangeSumBST_SevenNodeTree_SumsValuesInsideRange()
    {
        // [10,5,15,3,7,null,18], low=7, high=15 -> 7 + 10 + 15 = 32
        var root = new BinaryTreeNode<int>(10)
        {
            Left = new(5) { Left = new(3), Right = new(7) },
            Right = new(15) { Right = new(18) },
        };

        Assert.Equal(32, RangeSumBST(root, 7, 15));
    }

    [Fact]
    public void RangeSumBST_TenNodeTree_SumsValuesInsideRange()
    {
        // [10,5,15,3,7,13,18,1,null,6], low=6, high=10 -> 6 + 7 + 10 = 23
        var root = new BinaryTreeNode<int>(10)
        {
            Left = new(5) { Left = new(3) { Left = new(1) }, Right = new(7) { Left = new(6) } },
            Right = new(15) { Left = new(13), Right = new(18) },
        };

        Assert.Equal(23, RangeSumBST(root, 6, 10));
    }

    [Fact]
    public void RangeSumBST_RangeOutsideEntireTree_ReturnsZero()
    {
        var root = new BinaryTreeNode<int>(10) { Left = new(5), Right = new(15) };

        Assert.Equal(0, RangeSumBST(root, 100, 200));
    }

    private static int RangeSumBST(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return 0;
        }

        if (node.Value < low)
        {
            return RangeSumBST(node.Right, low, high);
        }

        if (node.Value > high)
        {
            return RangeSumBST(node.Left, low, high);
        }

        return node.Value + RangeSumBST(node.Left, low, high) + RangeSumBST(node.Right, low, high);
    }
}
