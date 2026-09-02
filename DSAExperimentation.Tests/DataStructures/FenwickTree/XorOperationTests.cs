using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class XorOperationTests
{
    private const int CombineLeftValue = 5;
    private const int CombineRightValue = 3;
    private const int CombineExpectedXor = 6;

    private const int InvertValue = 5;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, XorOperation<int>.Identity);

    [Fact]
    public void Combine_XorsBothValues()
    {
        var actual = XorOperation<int>.Combine(CombineLeftValue, CombineRightValue);
        Assert.Equal(CombineExpectedXor, actual);
    }

    [Fact]
    public void Invert_IsSelfInverse() => Assert.Equal(InvertValue, XorOperation<int>.Invert(InvertValue));

    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity()
    {
        var actual = XorOperation<int>.Combine(InvertValue, XorOperation<int>.Invert(InvertValue));
        Assert.Equal(XorOperation<int>.Identity, actual);
    }
}
