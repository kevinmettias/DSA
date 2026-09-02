using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.Tests.DataStructures.RangeFenwickTree;

public sealed partial class ScaledSumOperationTests
{
    private const int CombineLeftValue = 3;
    private const int CombineRightValue = 4;
    private const int CombineExpectedSum = 7;

    private const int ScaleValue = 4;
    private const int ScaleCount = 5;
    private const int ScaleExpectedProduct = 20;

    private const int InvertValue = 5;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, ScaledSumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues()
    {
        var actual = ScaledSumOperation<int>.Combine(CombineLeftValue, CombineRightValue);
        Assert.Equal(CombineExpectedSum, actual);
    }

    [Fact]
    public void Scale_MultipliesValueByCount()
    {
        var actual = ScaledSumOperation<int>.Scale(ScaleValue, ScaleCount);
        Assert.Equal(ScaleExpectedProduct, actual);
    }

    [Fact]
    public void Scale_ByZero_ReturnsIdentity()
    {
        var actual = ScaledSumOperation<int>.Scale(ScaleValue, 0);
        Assert.Equal(ScaledSumOperation<int>.Identity, actual);
    }

    [Fact]
    public void Invert_NegatesValue()
    {
        var actual = ScaledSumOperation<int>.Invert(InvertValue);
        Assert.Equal(-InvertValue, actual);
    }
}
