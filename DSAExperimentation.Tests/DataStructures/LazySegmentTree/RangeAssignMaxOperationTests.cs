using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class RangeAssignMaxOperationTests
{
    private const int SmallerOperand = 3;
    private const int LargerOperand = 4;
    private const int ExistingAggregate = 3;
    private const int AssignedValue = 9;
    private const int IrrelevantRangeLengthWhenAssigning = 100;
    private const int IrrelevantRangeLengthWhenUnchanged = 5;
    private const int NewerUpdate = 9;
    private const int OlderUpdate = 4;

    [Fact]
    public void Identity_IsMinValue() => Assert.Equal(int.MinValue, RangeAssignMaxOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsLargerValue()
    {
        var actual = RangeAssignMaxOperation<int>.Combine(SmallerOperand, LargerOperand);
        Assert.Equal(LargerOperand, actual);
    }

    [Fact]
    public void ApplyUpdate_PendingAssignment_ReturnsAssignedValueRegardlessOfRangeLength()
    {
        var actual = RangeAssignMaxOperation<int>.ApplyUpdate(
            ExistingAggregate, AssignedValue, IrrelevantRangeLengthWhenAssigning);
        Assert.Equal(AssignedValue, actual);
    }

    [Fact]
    public void ApplyUpdate_NoUpdate_ReturnsAggregateUnchanged()
    {
        var actual = RangeAssignMaxOperation<int>.ApplyUpdate(
            ExistingAggregate, RangeAssignMaxOperation<int>.NoUpdate, IrrelevantRangeLengthWhenUnchanged);
        Assert.Equal(ExistingAggregate, actual);
    }

    [Fact]
    public void ComposeUpdate_NewerAssignmentReplacesOlderOne()
    {
        var actual = RangeAssignMaxOperation<int>.ComposeUpdate(NewerUpdate, OlderUpdate);
        Assert.Equal(NewerUpdate, actual);
    }

    [Fact]
    public void NoUpdate_IsNull() => Assert.Null(RangeAssignMaxOperation<int>.NoUpdate);
}
