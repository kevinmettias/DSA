using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.Tests.DataStructures.MonotonicDeque;

public sealed partial class MinWindowOrderTests
{
    [Fact]
    public void IsDominatedBy_SmallerArriving_ReturnsTrue() =>
        Assert.True(MinWindowOrder<int>.IsDominatedBy(3, 2));

    [Fact]
    public void IsDominatedBy_EqualArriving_ReturnsTrue() =>
        Assert.True(MinWindowOrder<int>.IsDominatedBy(4, 4));

    [Fact]
    public void IsDominatedBy_LargerArriving_ReturnsFalse() =>
        Assert.False(MinWindowOrder<int>.IsDominatedBy(3, 5));
}
