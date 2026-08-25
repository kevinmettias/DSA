using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.DataStructures.Heap;

public sealed partial class HeapArrayIndexTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 1)]
    [InlineData(4, 1)]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    public void Parent_OfChildIndex_ReturnsExpectedParentIndex(int childIndex, int expectedParent)
    {
        Assert.Equal(expectedParent, HeapArrayIndex.Parent(childIndex));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 3)]
    [InlineData(2, 5)]
    public void LeftChild_OfParentIndex_ReturnsExpectedIndex(int parentIndex, int expectedLeftChild)
    {
        Assert.Equal(expectedLeftChild, HeapArrayIndex.LeftChild(parentIndex));
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 4)]
    [InlineData(2, 6)]
    public void RightChild_OfParentIndex_ReturnsExpectedIndex(int parentIndex, int expectedRightChild)
    {
        Assert.Equal(expectedRightChild, HeapArrayIndex.RightChild(parentIndex));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(42)]
    public void Parent_OfLeftAndRightChildOfIndex_RoundTripsToOriginalIndex(int index)
    {
        Assert.Equal(index, HeapArrayIndex.Parent(HeapArrayIndex.LeftChild(index)));
        Assert.Equal(index, HeapArrayIndex.Parent(HeapArrayIndex.RightChild(index)));
    }
}
