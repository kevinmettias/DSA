using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class SumOperationTests
{
    private const int CombineLeftValue = 3;
    private const int CombineRightValue = 4;
    private const int CombineExpectedSum = 7;

    private const int InvertValue = 5;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, SumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues()
    {
        var actual = SumOperation<int>.Combine(CombineLeftValue, CombineRightValue);
        Assert.Equal(CombineExpectedSum, actual);
    }

    [Fact]
    public void Invert_NegatesValue() => Assert.Equal(-InvertValue, SumOperation<int>.Invert(InvertValue));

    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity()
    {
        var actual = SumOperation<int>.Combine(InvertValue, SumOperation<int>.Invert(InvertValue));
        Assert.Equal(SumOperation<int>.Identity, actual);
    }
}
