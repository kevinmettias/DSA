using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.DataStructures.LazySegmentTree;

public sealed partial class RangeAddSumOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues()
    {
        var actual = RangeAddSumOperation<int>.Combine(Combine.LeftValue, Combine.RightValue);

        Assert.Equal(Combine.ExpectedSum, actual);
    }

    [Fact]
    public void ApplyUpdate_AddsDeltaTimesRangeLengthToAggregate()
    {
        var actual = RangeAddSumOperation<int>.ApplyUpdate(
            ApplyUpdate.Aggregate, ApplyUpdate.Delta, ApplyUpdate.RangeLength);

        Assert.Equal(ApplyUpdate.ExpectedAggregate, actual);
    }

    [Fact]
    public void ComposeUpdate_AccumulatesBothPendingDeltas()
    {
        var actual = RangeAddSumOperation<int>.ComposeUpdate(ComposeUpdate.OuterDelta, ComposeUpdate.InnerDelta);

        Assert.Equal(ComposeUpdate.ExpectedDelta, actual);
    }

    [Fact]
    public void NoUpdate_IsZero() => Assert.Equal(0, RangeAddSumOperation<int>.NoUpdate);

    /// <summary>
    /// The two operands these tests combine and the sum they expect back.
    /// </summary>
    private static class Combine
    {
        public const int LeftValue = 3;
        public const int RightValue = 4;
        public const int ExpectedSum = 7;
    }

    /// <summary>
    /// One pending update: the aggregate it lands on, the delta, the range length it is
    /// multiplied by, and the aggregate the three add up to.
    /// </summary>
    private static class ApplyUpdate
    {
        public const int Aggregate = 10;
        public const int Delta = 2;
        public const int RangeLength = 5;
        public const int ExpectedAggregate = 20;
    }

    /// <summary>
    /// Two updates composed into one, and the delta they accumulate to.
    /// </summary>
    private static class ComposeUpdate
    {
        public const int OuterDelta = 3;
        public const int InnerDelta = 4;
        public const int ExpectedDelta = 7;
    }
}
