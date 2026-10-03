using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class GcdOperationTests
{
    private const int First = 12;
    private const int Second = 18;
    private const int Third = 8;
    private const int ExpectedGcd = 6;
    private const int ExpectedGcdOfAllThree = 2;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, GcdOperation<int>.Identity);

    [Fact]
    public void Combine_IsTheGreatestCommonDivisor() =>
        Assert.Equal(ExpectedGcd, GcdOperation<int>.Combine(First, Second));

    // The identity law a segment tree's "no overlap" branch relies on: folding the identity in
    // changes nothing.
    [Fact]
    public void Combine_WithIdentity_ReturnsTheOtherValue() =>
        Assert.Equal(First, GcdOperation<int>.Combine(GcdOperation<int>.Identity, First));

    // The associativity a segment tree's arbitrary split points rely on.
    [Fact]
    public void Combine_IsAssociative() =>
        Assert.Equal(
            GcdOperation<long>.Combine(GcdOperation<long>.Combine(First, Second), Third),
            GcdOperation<long>.Combine(First, GcdOperation<long>.Combine(Second, Third)));

    [Fact]
    public void Combine_OfThree_IsTheirCommonDivisor() =>
        Assert.Equal(ExpectedGcdOfAllThree, GcdOperation<int>.Combine(GcdOperation<int>.Combine(First, Second), Third));
}
