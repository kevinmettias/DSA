using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class GreatestCommonDivisorTests
{
    public static TheoryData<int, int, int> IntCasesToExpectedDivisors =>
        new() { { 12, 18, 6 }, { 18, 12, 6 }, { 7, 13, 1 }, { 0, 9, 9 }, { 9, 0, 9 }, { 0, 0, 0 }, { 1, 1, 1 } };

    public static TheoryData<int, int> SignedPairsOfTwelveAndEighteen =>
        new() { { -12, 18 }, { 12, -18 }, { -12, -18 } };

    [Theory]
    [MemberData(nameof(IntCasesToExpectedDivisors))]
    public void Of_IntPairs_ReturnsTheGreatestCommonDivisor(int first, int second, int expected) =>
        Assert.Equal(expected, GreatestCommonDivisor.Of(first, second));

    // Callers reducing a signed slope or difference rely on a non-negative divisor.
    [Theory]
    [MemberData(nameof(SignedPairsOfTwelveAndEighteen))]
    public void Of_NegativeInputs_ReturnsThePositiveDivisor(int first, int second) =>
        Assert.Equal(6, GreatestCommonDivisor.Of(first, second));

    [Fact]
    public void Of_LongInputsPastIntRange_ReturnsTheDivisor() =>
        Assert.Equal(3_000_000_000L, GreatestCommonDivisor.Of(6_000_000_000L, 9_000_000_000L));

    [Fact]
    public void Of_IntMinValue_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => GreatestCommonDivisor.Of(int.MinValue, 2));
}
