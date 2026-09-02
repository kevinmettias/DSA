using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class MinOperationTests
{
    private const int SmallerValue = 3;
    private const int LargerValue = 4;
    private const int SoleValue = 5;

    [Fact]
    public void Identity_IsMaxValue() => Assert.Equal(int.MaxValue, MinOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsSmallerValue()
    {
        var actual = MinOperation<int>.Combine(SmallerValue, LargerValue);
        Assert.Equal(SmallerValue, actual);
    }

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
    {
        var actual = MinOperation<int>.Combine(MinOperation<int>.Identity, SoleValue);
        Assert.Equal(SoleValue, actual);
    }
}
