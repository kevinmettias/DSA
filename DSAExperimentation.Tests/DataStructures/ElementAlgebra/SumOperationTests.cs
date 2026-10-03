using DSAExperimentation.DataStructures.ElementAlgebra;

namespace DSAExperimentation.Tests.DataStructures.ElementAlgebra;

// SumOperation serves three positions of the element-algebra chain at once - monoid for SegmentTree,
// group for FenwickTree, scaled group for RangeFenwickTree - so each position's law is pinned here.
public sealed partial class SumOperationTests
{
    private const int FirstAddend = 3;
    private const int SecondAddend = 4;
    private const int ExpectedSum = 7;
    private const int SoleValue = 5;
    private const int ScaleCount = 5;
    private const int ExpectedScaledProduct = 20;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, SumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(ExpectedSum, SumOperation<int>.Combine(FirstAddend, SecondAddend));

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged() =>
        Assert.Equal(SoleValue, SumOperation<int>.Combine(SumOperation<int>.Identity, SoleValue));

    [Fact]
    public void Invert_NegatesValue() => Assert.Equal(-SoleValue, SumOperation<int>.Invert(SoleValue));

    // The group law FenwickTree's range query rests on: a value combined with its inverse is the identity.
    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity() =>
        Assert.Equal(SumOperation<int>.Identity, SumOperation<int>.Combine(SoleValue, SumOperation<int>.Invert(SoleValue)));

    // The scaled-group law RangeFenwickTree rests on: Scale(value, count) equals combining value count times.
    [Fact]
    public void Scale_EqualsRepeatedCombine()
    {
        var repeated = Enumerable.Repeat(SecondAddend, ScaleCount).Aggregate(SumOperation<int>.Identity, SumOperation<int>.Combine);

        Assert.Equal(ExpectedScaledProduct, SumOperation<int>.Scale(SecondAddend, ScaleCount));
        Assert.Equal(repeated, SumOperation<int>.Scale(SecondAddend, ScaleCount));
    }

    [Fact]
    public void Scale_ByZero_ReturnsIdentity() => Assert.Equal(SumOperation<int>.Identity, SumOperation<int>.Scale(SecondAddend, 0));
}
