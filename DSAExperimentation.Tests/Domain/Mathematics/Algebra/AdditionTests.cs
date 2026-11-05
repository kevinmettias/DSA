using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.Tests.Domain.Mathematics.Algebra;

// Addition serves three positions of the element-algebra chain at once - monoid for SegmentTree,
// abelian group for FenwickTree, integer module for RangeFenwickTree - so each position's law is
// pinned here.
public sealed partial class AdditionTests
{
    private const int FirstAddend = 3;
    private const int SecondAddend = 4;
    private const int ExpectedSum = 7;
    private const int SoleValue = 5;
    private const int ScaleCount = 5;
    private const int ExpectedScaledProduct = 20;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, Addition<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(ExpectedSum, Addition<int>.Combine(FirstAddend, SecondAddend));

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged() =>
        Assert.Equal(SoleValue, Addition<int>.Combine(Addition<int>.Identity, SoleValue));

    [Fact]
    public void Invert_NegatesValue() => Assert.Equal(-SoleValue, Addition<int>.Invert(SoleValue));

    // The group law FenwickTree's range query rests on: a value combined with its inverse is the identity.
    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity() =>
        Assert.Equal(Addition<int>.Identity, Addition<int>.Combine(SoleValue, Addition<int>.Invert(SoleValue)));

    // The scaled-group law RangeFenwickTree rests on: Scale(value, count) equals combining value count times.
    [Fact]
    public void Scale_EqualsRepeatedCombine()
    {
        var repeated = Enumerable.Repeat(SecondAddend, ScaleCount).Aggregate(Addition<int>.Identity, Addition<int>.Combine);

        Assert.Equal(ExpectedScaledProduct, Addition<int>.Scale(SecondAddend, ScaleCount));
        Assert.Equal(repeated, Addition<int>.Scale(SecondAddend, ScaleCount));
    }

    [Fact]
    public void Scale_ByZero_ReturnsIdentity() => Assert.Equal(Addition<int>.Identity, Addition<int>.Scale(SecondAddend, 0));
}
