using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class RangeAddSumOperationTests
{
    private const int CombineLeftValue = 3;
    private const int CombineRightValue = 4;
    private const int CombineExpectedSum = 7;

    private const int ApplyUpdateAggregate = 10;
    private const int ApplyUpdateDelta = 2;
    private const int ApplyUpdateRangeLength = 5;
    private const int ApplyUpdateExpectedAggregate = 20;

    private const int ComposeUpdateOuterDelta = 3;
    private const int ComposeUpdateInnerDelta = 4;
    private const int ComposeUpdateExpectedDelta = 7;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues()
    {
        var actual = RangeAddSumOperation<int>.Combine(CombineLeftValue, CombineRightValue);
        Assert.Equal(CombineExpectedSum, actual);
    }

    [Fact]
    public void ApplyUpdate_AddsDeltaTimesRangeLengthToAggregate()
    {
        var actual = RangeAddSumOperation<int>.ApplyUpdate(ApplyUpdateAggregate, ApplyUpdateDelta, ApplyUpdateRangeLength);
        Assert.Equal(ApplyUpdateExpectedAggregate, actual);
    }

    [Fact]
    public void ComposeUpdate_AccumulatesBothPendingDeltas()
    {
        var actual = RangeAddSumOperation<int>.ComposeUpdate(ComposeUpdateOuterDelta, ComposeUpdateInnerDelta);
        Assert.Equal(ComposeUpdateExpectedDelta, actual);
    }

    [Fact]
    public void NoUpdate_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.NoUpdate);
}
