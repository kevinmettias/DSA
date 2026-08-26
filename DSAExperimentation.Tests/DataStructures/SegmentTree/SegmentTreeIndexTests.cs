using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeIndexTests
{
    public static TheoryData<int, int> NodeToExpectedLeftChild => new()
    {
        { 0, 1 },
        { 1, 3 },
        { 2, 5 },
    };

    [Theory]
    [MemberData(nameof(NodeToExpectedLeftChild))]
    public void LeftChild_OfNode_ReturnsExpectedIndex(int node, int expectedLeftChild) => Assert.Equal(expectedLeftChild, SegmentTreeIndex.LeftChild(node));

    public static TheoryData<int, int> NodeToExpectedRightChild => new()
    {
        { 0, 2 },
        { 1, 4 },
        { 2, 6 },
    };

    [Theory]
    [MemberData(nameof(NodeToExpectedRightChild))]
    public void RightChild_OfNode_ReturnsExpectedIndex(int node, int expectedRightChild) => Assert.Equal(expectedRightChild, SegmentTreeIndex.RightChild(node));

    [Fact]
    public void RightChild_IsAlwaysLeftChildPlusOne()
    {
        foreach (var node in new[] { 0, 1, 5, 42 })
        {
            Assert.Equal(SegmentTreeIndex.LeftChild(node) + 1, SegmentTreeIndex.RightChild(node));
        }
    }
}
