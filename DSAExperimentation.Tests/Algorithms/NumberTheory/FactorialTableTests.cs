using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

// The table read modulo 10^9 + 7, the prime LeetCode reports in, and modulo 7, small enough to check
// by hand.
public sealed partial class FactorialTableTests
{
    private const long Prime = BillionSeven.Value;

    public static TheoryData<int, long> ArgumentsToExpectedFactorials =>
        new() { { 0, 1 }, { 1, 1 }, { 2, 2 }, { 5, 120 }, { 10, 3_628_800 } };

    public static TheoryData<int> ArgumentsForInverseFactorial =>
        new() { 0, 1, 7, 1_000 };

    public static TheoryData<int, int, long> BinomialCasesToKnownCoefficients =>
        new() { { 5, 2, 10 }, { 10, 3, 120 }, { 4, 4, 1 }, { 0, 0, 1 }, { 20, 10, 184_756 } };

    public static TheoryData<int, int> BinomialCasesOutsideTheRange =>
        new() { { 4, 5 }, { 4, -1 }, { -1, 0 }, { 0, 1 } };

    [Theory]
    [MemberData(nameof(ArgumentsToExpectedFactorials))]
    public void Factorial_MatchesOrdinaryFactorial(int argument, long expected) =>
        Assert.Equal(expected, FactorialTable<BillionSeven>.Build(argument).Factorial(argument));

    [Theory]
    [MemberData(nameof(ArgumentsForInverseFactorial))]
    public void InverseFactorial_MultipliedByItsFactorial_IsOne(int argument)
    {
        var table = FactorialTable<BillionSeven>.Build(argument);

        Assert.Equal(1, table.Factorial(argument) * table.InverseFactorial(argument) % Prime);
    }

    [Fact]
    public void InverseFactorial_EveryEntryBelowMaxN_InvertsItsFactorial()
    {
        var table = FactorialTable<BillionSeven>.Build(50);

        for (var n = 0; n <= 50; n++)
        {
            Assert.Equal(1, table.Factorial(n) * table.InverseFactorial(n) % Prime);
        }
    }

    // The table's reason for existing: one modular inverse at build time makes every
    // later division a multiplication.
    [Theory]
    [MemberData(nameof(BinomialCasesToKnownCoefficients))]
    public void Choose_MatchesKnownBinomialCoefficients(int totalCount, int chosenCount, long expected) =>
        Assert.Equal(expected, FactorialTable<BillionSeven>.Build(totalCount).Choose(totalCount, chosenCount));

    [Theory]
    [MemberData(nameof(BinomialCasesOutsideTheRange))]
    public void Choose_OutsideTheBinomialRange_IsZero(int totalCount, int chosenCount) =>
        Assert.Equal(0, FactorialTable<BillionSeven>.Build(4).Choose(totalCount, chosenCount));

    [Fact]
    public void Choose_IsTheFactorialOverTheTwoInverseFactorials()
    {
        var table = FactorialTable<BillionSeven>.Build(12);

        for (var n = 0; n <= 12; n++)
        {
            for (var r = 0; r <= n; r++)
            {
                var byDefinition = table.Factorial(n) * table.InverseFactorial(r) % Prime
                    * table.InverseFactorial(n - r) % Prime;

                Assert.Equal(byDefinition, table.Choose(n, r));
            }
        }
    }

    [Fact]
    public void Choose_EveryRowSummed_IsAPowerOfTwo()
    {
        var table = FactorialTable<BillionSeven>.Build(16);
        var rowSum = 0L;

        for (var r = 0; r <= 16; r++)
        {
            rowSum = (rowSum + table.Choose(16, r)) % Prime;
        }

        Assert.Equal(65_536, rowSum);
    }

    [Fact]
    public void Build_ZeroMaxN_YieldsATableOfTheSingleEmptyChoice()
    {
        var table = FactorialTable<BillionSeven>.Build(0);

        Assert.Equal(1, table.Factorial(0));
        Assert.Equal(1, table.InverseFactorial(0));
        Assert.Equal(1, table.Choose(0, 0));
    }

    // 5! is 120, which is 1 modulo 7, so its inverse is 1 too; 6! is 720, which is 6, its own inverse.
    [Fact]
    public void Build_ReducesModuloItsWitnessesPrime()
    {
        var table = FactorialTable<Seven>.Build(6);

        Assert.Equal(1, table.Factorial(5));
        Assert.Equal(6, table.Factorial(6));
        Assert.Equal(6, table.InverseFactorial(6));
    }

    private readonly struct BillionSeven : IPrimeModulus
    {
        public const long Value = 1_000_000_007;

        static long IModulus.Value => Value;
    }

    private readonly struct Seven : IPrimeModulus
    {
        public static long Value => 7;
    }
}
