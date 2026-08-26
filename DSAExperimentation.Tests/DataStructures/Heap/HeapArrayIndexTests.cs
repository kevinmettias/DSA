using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.DataStructures.Heap;

public sealed partial class HeapArrayIndexTests
{
    public static TheoryData<int, int> ChildIndexToExpectedParent => new()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 1 },
        { 4, 1 },
        { 5, 2 },
        { 6, 2 },
    };

    [Theory]
    [MemberData(nameof(ChildIndexToExpectedParent))]
    public void Parent_OfChildIndex_ReturnsExpectedParentIndex(int childIndex, int expectedParent) => Assert.Equal(expectedParent, HeapArrayIndex.Parent(childIndex));

    public static TheoryData<int, int> ParentIndexToExpectedLeftChild => new()
    {
        { 0, 1 },
        { 1, 3 },
        { 2, 5 },
    };

    [Theory]
    [MemberData(nameof(ParentIndexToExpectedLeftChild))]
    public void LeftChild_OfParentIndex_ReturnsExpectedIndex(int parentIndex, int expectedLeftChild) => Assert.Equal(expectedLeftChild, HeapArrayIndex.LeftChild(parentIndex));

    public static TheoryData<int, int> ParentIndexToExpectedRightChild => new()
    {
        { 0, 2 },
        { 1, 4 },
        { 2, 6 },
    };

    [Theory]
    [MemberData(nameof(ParentIndexToExpectedRightChild))]
    public void RightChild_OfParentIndex_ReturnsExpectedIndex(int parentIndex, int expectedRightChild) => Assert.Equal(expectedRightChild, HeapArrayIndex.RightChild(parentIndex));

    public static TheoryData<int> Indices => [0, 1, 5, 42];

    [Theory]
    [MemberData(nameof(Indices))]
    public void Parent_OfLeftAndRightChildOfIndex_RoundTripsToOriginalIndex(int index)
    {
        Assert.Equal(index, HeapArrayIndex.Parent(HeapArrayIndex.LeftChild(index)));
        Assert.Equal(index, HeapArrayIndex.Parent(HeapArrayIndex.RightChild(index)));
    }
}
