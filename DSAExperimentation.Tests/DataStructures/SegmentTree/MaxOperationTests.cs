using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class MaxOperationTests
{
    private const int SmallerValue = 3;
    private const int LargerValue = 4;
    private const int SoleValue = 5;

    [Fact]
    public void Identity_IsMinValue() => Assert.Equal(int.MinValue, MaxOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsLargerValue()
    {
        var actual = MaxOperation<int>.Combine(SmallerValue, LargerValue);
        Assert.Equal(LargerValue, actual);
    }

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
    {
        var actual = MaxOperation<int>.Combine(MaxOperation<int>.Identity, SoleValue);
        Assert.Equal(SoleValue, actual);
    }
}
