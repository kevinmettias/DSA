using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.Tests.DataStructures.MonotonicDeque;

public sealed partial class MaxWindowOrderTests
{
    [Fact]
    public void IsDominatedBy_LargerArriving_ReturnsTrue() =>
        Assert.True(MaxWindowOrder<int>.IsDominatedBy(3, 5));

    [Fact]
    public void IsDominatedBy_EqualArriving_ReturnsTrue() =>
        Assert.True(MaxWindowOrder<int>.IsDominatedBy(4, 4));

    [Fact]
    public void IsDominatedBy_SmallerArriving_ReturnsFalse() =>
        Assert.False(MaxWindowOrder<int>.IsDominatedBy(3, 2));

    // A tuple key orders by its first field, then its second - the tie-break a two-level window uses.
    [Fact]
    public void IsDominatedBy_TupleKey_BreaksTiesOnTheSecondField() =>
        Assert.False(MaxWindowOrder<(long Value, long Tiebreak)>.IsDominatedBy((7, 2), (7, 1)));
}
