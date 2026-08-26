using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class RangeAddSumOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(7, RangeAddSumOperation<int>.Combine(3, 4));

    [Fact]
    public void ApplyUpdate_AddsDeltaTimesRangeLengthToAggregate()
        => Assert.Equal(20, RangeAddSumOperation<int>.ApplyUpdate(10, 2, 5));

    [Fact]
    public void ComposeUpdate_AccumulatesBothPendingDeltas()
        => Assert.Equal(7, RangeAddSumOperation<int>.ComposeUpdate(3, 4));

    [Fact]
    public void NoUpdate_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.NoUpdate);
}
