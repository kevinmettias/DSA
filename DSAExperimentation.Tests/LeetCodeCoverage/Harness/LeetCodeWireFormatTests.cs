using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Harness;

// Every tree- and list-shaped problem's cases are stated in LeetCode's array
// notation and pass through here, so a bug in this translation would show up as a
// wrong answer in an unrelated problem's registration. The omitted-children rule
// is the part worth pinning down: LeetCode does NOT pad a missing node's slots,
// so the array is not a 2i+1/2i+2 heap layout and indexing it as one silently
// builds a different tree.
public sealed class LeetCodeWireFormatTests
{
    [Fact]
    public void ToBinaryTree_OnAnEmptyArray_ReturnsNull()
    {
        Assert.Null(LeetCodeWireFormat.ToBinaryTree([]));
    }

    [Fact]
    public void ToBinaryTree_OnANullRoot_ReturnsNull()
    {
        Assert.Null(LeetCodeWireFormat.ToBinaryTree([null]));
    }

    [Fact]
    public void ToBinaryTree_OnLeetCodesOwnExample_PlacesEveryNode()
    {
        var root = LeetCodeWireFormat.ToBinaryTree([3, 9, 20, null, null, 15, 7]);

        Assert.Equal(3, root!.Value);
        Assert.Equal(9, root.Left!.Value);
        Assert.Equal(20, root.Right!.Value);
        Assert.Null(root.Left.Left);
        Assert.Null(root.Left.Right);
        Assert.Equal(15, root.Right.Left!.Value);
        Assert.Equal(7, root.Right.Right!.Value);
    }

    // The case that separates a queue-driven read from a heap-index read: 9's
    // children are absent and OMITTED, so 15 belongs to 20, not to 9. A 2i+1
    // reading would hang 15 under 9 instead.
    [Fact]
    public void ToBinaryTree_WhenAnAbsentNodesChildrenAreOmitted_DoesNotShiftLaterNodes()
    {
        var root = LeetCodeWireFormat.ToBinaryTree([1, null, 2, 3]);

        Assert.Null(root!.Left);
        Assert.Equal(2, root.Right!.Value);
        Assert.Equal(3, root.Right.Left!.Value);
    }

    [Fact]
    public void ToLinkedList_ThenBack_RoundTripsTheValuesInOrder()
    {
        Assert.Equal([1, 2, 4], LeetCodeWireFormat.FromLinkedList(LeetCodeWireFormat.ToLinkedList([1, 2, 4])));
    }

    [Fact]
    public void ToLinkedList_OnAnEmptyArray_ReturnsNull()
    {
        Assert.Null(LeetCodeWireFormat.ToLinkedList([]));
        Assert.Empty(LeetCodeWireFormat.FromLinkedList(null));
    }
}
