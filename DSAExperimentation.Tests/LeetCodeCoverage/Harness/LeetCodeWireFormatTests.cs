using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Harness;

// Every tree- and list-shaped problem's cases are stated in LeetCode's array
// notation and pass through here, so a bug in this translation would show up as a
// wrong answer in an unrelated problem's registration. The omitted-children rule
// is the part worth pinning down: LeetCode does NOT pad a missing node's slots,
// so the array is not a 2i+1/2i+2 heap layout and indexing it as one silently
// builds a different tree.
public sealed partial class LeetCodeWireFormatTests
{
    [Fact]
    public void ToBinaryTree_OnAnEmptyArray_ReturnsNull() => Assert.Null(LeetCodeWireFormat.ToBinaryTree([]));

    [Fact]
    public void ToBinaryTree_OnANullRoot_ReturnsNull() => Assert.Null(LeetCodeWireFormat.ToBinaryTree([null]));

    [Fact]
    public void ToBinaryTree_OnLeetCodesOwnExample_PlacesEveryNode()
    {
        var root = Assert.IsType<BinaryTreeNode<int>>(
            LeetCodeWireFormat.ToBinaryTree([3, 9, 20, null, null, 15, 7]));
        var left = Assert.IsType<BinaryTreeNode<int>>(root.Left);
        var right = Assert.IsType<BinaryTreeNode<int>>(root.Right);

        Assert.Equal(3, root.Value);
        Assert.Equal(9, left.Value);
        Assert.Equal(20, right.Value);
        Assert.Null(left.Left);
        Assert.Null(left.Right);
        Assert.Equal(15, Assert.IsType<BinaryTreeNode<int>>(right.Left).Value);
        Assert.Equal(7, Assert.IsType<BinaryTreeNode<int>>(right.Right).Value);
    }

    // The case that separates a queue-driven read from a heap-index read: 9's
    // children are absent and OMITTED, so 15 belongs to 20, not to 9. A 2i+1
    // reading would hang 15 under 9 instead.
    [Fact]
    public void ToBinaryTree_WhenAnAbsentNodesChildrenAreOmitted_DoesNotShiftLaterNodes()
    {
        var root = Assert.IsType<BinaryTreeNode<int>>(LeetCodeWireFormat.ToBinaryTree([1, null, 2, 3]));
        var right = Assert.IsType<BinaryTreeNode<int>>(root.Right);

        Assert.Null(root.Left);
        Assert.Equal(2, right.Value);
        Assert.Equal(3, Assert.IsType<BinaryTreeNode<int>>(right.Left).Value);
    }

    [Fact]
    public void ToLinkedList_ThenBack_RoundTripsTheValuesInOrder() => Assert.Equal([1, 2, 4], LeetCodeWireFormat.FromLinkedList(LeetCodeWireFormat.ToLinkedList([1, 2, 4])));

    [Fact]
    public void ToLinkedList_OnAnEmptyArray_ReturnsNull()
    {
        Assert.Null(LeetCodeWireFormat.ToLinkedList([]));
        Assert.Empty(LeetCodeWireFormat.FromLinkedList(null));
    }
}
