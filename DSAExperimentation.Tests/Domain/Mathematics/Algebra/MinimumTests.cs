using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Tests.Domain.Mathematics.Algebra;

public sealed partial class MinimumTests
{
    private const int SmallerValue = 3;
    private const int LargerValue = 4;
    private const int SoleValue = 5;

    [Fact]
    public void Identity_IsMaxValue() => Assert.Equal(int.MaxValue, Minimum<int>.Identity);

    [Fact]
    public void Combine_ReturnsSmallerValue()
    {
        var actual = Minimum<int>.Combine(SmallerValue, LargerValue);
        Assert.Equal(SmallerValue, actual);
    }

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
    {
        var actual = Minimum<int>.Combine(Minimum<int>.Identity, SoleValue);
        Assert.Equal(SoleValue, actual);
    }
}
