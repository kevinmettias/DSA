using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class ModularPowerTests
{
    private const long SuperPowModulus = 1_337;

    public static TheoryData<long, long, long, long> CasesToExpectedPowers =>
        new() { { 2, 10, 1_000, 24 }, { 3, 4, 7, 4 }, { 2, 0, 7, 1 }, { 0, 3, 7, 0 }, { 10, 3, 1_337, 1_000 } };

    [Theory]
    [MemberData(nameof(CasesToExpectedPowers))]
    public void Of_SmallCases_MatchesOrdinaryExponentiationModulo(long value, long exponent, long modulus, long expected) =>
        Assert.Equal(expected, ModularPower.Of(value, exponent, modulus));

    // Any number mod 1 is 0, including the empty product an exponent of 0 gives.
    [Fact]
    public void Of_ZeroExponentModuloOne_IsZero() => Assert.Equal(0, ModularPower.Of(5, 0, 1));

    [Fact]
    public void Of_NegativeBase_KeepsANegativeRemainder() => Assert.Equal(-8, ModularPower.Of(-2, 3, 1_000));

    // Fermat over a non-LeetCode prime modulus: the algorithm does not depend on 1e9+7.
    [Fact]
    public void Of_PrimeModulus_SatisfiesFermat() => Assert.Equal(1, ModularPower.Of(7, 997 - 1, 997));

    [Fact]
    public void Of_LargeExponent_StaysBelowTheModulus() =>
        Assert.InRange(ModularPower.Of(123_456_789, 1_000_000_000_000, SuperPowModulus), 0, SuperPowModulus - 1);
}
