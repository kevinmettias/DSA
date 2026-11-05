using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Tests.Domain.Mathematics.Algebra;

public sealed partial class MaximumTests
{
    private const int SmallerValue = 3;
    private const int LargerValue = 4;
    private const int SoleValue = 5;

    [Fact]
    public void Identity_IsMinValue() => Assert.Equal(int.MinValue, Maximum<int>.Identity);

    [Fact]
    public void Combine_ReturnsLargerValue()
    {
        var actual = Maximum<int>.Combine(SmallerValue, LargerValue);
        Assert.Equal(LargerValue, actual);
    }

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
    {
        var actual = Maximum<int>.Combine(Maximum<int>.Identity, SoleValue);
        Assert.Equal(SoleValue, actual);
    }
}
