using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class RangeAssignMaxOperationTests
{
    [Fact]
    public void Identity_IsMinValue() => Assert.Equal(int.MinValue, RangeAssignMaxOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsLargerValue() => Assert.Equal(4, RangeAssignMaxOperation<int>.Combine(3, 4));

    [Fact]
    public void ApplyUpdate_PendingAssignment_ReturnsAssignedValueRegardlessOfRangeLength()
        => Assert.Equal(9, RangeAssignMaxOperation<int>.ApplyUpdate(3, 9, 100));

    [Fact]
    public void ApplyUpdate_NoUpdate_ReturnsAggregateUnchanged()
        => Assert.Equal(3, RangeAssignMaxOperation<int>.ApplyUpdate(3, RangeAssignMaxOperation<int>.NoUpdate, 5));

    [Fact]
    public void ComposeUpdate_NewerAssignmentReplacesOlderOne()
        => Assert.Equal(9, RangeAssignMaxOperation<int>.ComposeUpdate(9, 4));

    [Fact]
    public void NoUpdate_IsNull() => Assert.Null(RangeAssignMaxOperation<int>.NoUpdate);
}
