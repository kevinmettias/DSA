using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Tests.Domain.Mathematics.Algebra;

public sealed partial class BitwiseXorTests
{
    private const int CombineLeftValue = 5;
    private const int CombineRightValue = 3;
    private const int CombineExpectedXor = 6;

    private const int InvertValue = 5;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, BitwiseXor<int>.Identity);

    [Fact]
    public void Combine_XorsBothValues()
    {
        var actual = BitwiseXor<int>.Combine(CombineLeftValue, CombineRightValue);
        Assert.Equal(CombineExpectedXor, actual);
    }

    [Fact]
    public void Invert_IsSelfInverse() => Assert.Equal(InvertValue, BitwiseXor<int>.Invert(InvertValue));

    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity()
    {
        var actual = BitwiseXor<int>.Combine(InvertValue, BitwiseXor<int>.Invert(InvertValue));
        Assert.Equal(BitwiseXor<int>.Identity, actual);
    }
}
