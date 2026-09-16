using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.Tests.Domain.Modular;

public sealed class ModularArithmeticTests
{
    [Theory]
    [InlineData(2, 0, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 10, 1024)]
    [InlineData(3, 5, 243)]
    [InlineData(0, 5, 0)]
    [InlineData(1, 1_000_000, 1)]
    public void Power_SmallCases_MatchesOrdinaryExponentiation(long value, long exponent, long expected)
    {
        var power = ModularArithmetic.Power(value, exponent);

        Assert.Equal(expected, power);
    }

    [Fact]
    public void Power_ResultAlwaysStaysBelowTheModulus() => Assert.True(ModularArithmetic.Power(999_999_999, 12) < ModularArithmetic.Modulo);

    [Fact]
    public void Power_LargeExponent_DoesNotOverflow()
    {
        // Fermat: a^(p-1) == 1 (mod p) for prime p and a not divisible by p.
        var power = ModularArithmetic.Power(7, ModularArithmetic.Modulo - 1);

        Assert.Equal(1, power);
    }

    [Fact]
    public void Power_ReducesItsBaseModuloFirst()
    {
        var reduced = ModularArithmetic.Power(5, 7);
        var unreduced = ModularArithmetic.Power(5 + ModularArithmetic.Modulo, 7);

        Assert.Equal(reduced, unreduced);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(1_000)]
    [InlineData(999_999_937)]
    public void Inverse_MultipliedByItsInput_IsOne(long value) =>
        Assert.Equal(1, value % ModularArithmetic.Modulo * ModularArithmetic.Inverse(value) % ModularArithmetic.Modulo);

    [Fact]
    public void Inverse_OfOne_IsOne() => Assert.Equal(1, ModularArithmetic.Inverse(1));

    [Fact]
    public void Inverse_DividesExactlyWhereOrdinaryDivisionWould()
    {
        // 20 / 4 == 5, computed as 20 * inverse(4) mod p.
        var quotient = 20 * ModularArithmetic.Inverse(4) % ModularArithmetic.Modulo;

        Assert.Equal(5, quotient);
    }

    [Fact]
    public void Modulo_IsTheLeetCodePrime() => Assert.Equal(1_000_000_007, ModularArithmetic.Modulo);
}
