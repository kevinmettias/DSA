using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.Tests.Domain.Modular;

public sealed class FactorialTableTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(5, 120)]
    [InlineData(10, 3_628_800)]
    public void Factorial_MatchesOrdinaryFactorial(int argument, long expected) =>
        Assert.Equal(expected, FactorialTable.Build(argument).Factorial(argument));

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(1_000)]
    public void InverseFactorial_MultipliedByItsFactorial_IsOne(int argument)
    {
        var table = FactorialTable.Build(argument);

        Assert.Equal(1, table.Factorial(argument) * table.InverseFactorial(argument) % ModularArithmetic.Modulo);
    }

    [Fact]
    public void InverseFactorial_EveryEntryBelowMaxN_InvertsItsFactorial()
    {
        var table = FactorialTable.Build(50);

        for (var n = 0; n <= 50; n++)
        {
            Assert.Equal(1, table.Factorial(n) * table.InverseFactorial(n) % ModularArithmetic.Modulo);
        }
    }

    // The table's reason for existing: one modular inverse at build time makes every
    // later division a multiplication.
    [Theory]
    [InlineData(5, 2, 10)]
    [InlineData(10, 3, 120)]
    [InlineData(4, 4, 1)]
    [InlineData(0, 0, 1)]
    [InlineData(20, 10, 184_756)]
    public void Choose_MatchesKnownBinomialCoefficients(int totalCount, int chosenCount, long expected) =>
        Assert.Equal(expected, FactorialTable.Build(totalCount).Choose(totalCount, chosenCount));

    [Theory]
    [InlineData(4, 5)]
    [InlineData(4, -1)]
    [InlineData(-1, 0)]
    [InlineData(0, 1)]
    public void Choose_OutsideTheBinomialRange_IsZero(int totalCount, int chosenCount) =>
        Assert.Equal(0, FactorialTable.Build(4).Choose(totalCount, chosenCount));

    [Fact]
    public void Choose_IsTheFactorialOverTheTwoInverseFactorials()
    {
        var table = FactorialTable.Build(12);

        for (var n = 0; n <= 12; n++)
        {
            for (var r = 0; r <= n; r++)
            {
                var byDefinition = table.Factorial(n) * table.InverseFactorial(r) % ModularArithmetic.Modulo
                    * table.InverseFactorial(n - r) % ModularArithmetic.Modulo;

                Assert.Equal(byDefinition, table.Choose(n, r));
            }
        }
    }

    [Fact]
    public void Choose_EveryRowSummed_IsAPowerOfTwo()
    {
        var table = FactorialTable.Build(16);
        var rowSum = 0L;

        for (var r = 0; r <= 16; r++)
        {
            rowSum = (rowSum + table.Choose(16, r)) % ModularArithmetic.Modulo;
        }

        Assert.Equal(65_536, rowSum);
    }

    [Fact]
    public void Build_ZeroMaxN_YieldsATableOfTheSingleEmptyChoice()
    {
        var table = FactorialTable.Build(0);

        Assert.Equal(1, table.Factorial(0));
        Assert.Equal(1, table.InverseFactorial(0));
        Assert.Equal(1, table.Choose(0, 0));
    }
}
