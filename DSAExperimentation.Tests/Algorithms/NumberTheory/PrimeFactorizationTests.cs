using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class PrimeFactorizationTests
{
    public static TheoryData<int> ValuesBelowTwo => new() { 0, 1 };

    [Fact]
    public void Distinct_RepeatedFactors_YieldsEachPrimeOnceAscending() =>
        Assert.Equal([2, 3], PrimeFactorization.Distinct(72));

    [Fact]
    public void Distinct_Prime_YieldsItself() => Assert.Equal([97], PrimeFactorization.Distinct(97));

    // The factor left after trial division ends is itself prime and larger than any tried.
    [Fact]
    public void Distinct_LargePrimeCofactor_YieldsItLast() =>
        Assert.Equal([2, 1_000_003], PrimeFactorization.Distinct(2 * 1_000_003));

    [Theory]
    [MemberData(nameof(ValuesBelowTwo))]
    public void Distinct_BelowTwo_YieldsNothing(int value) => Assert.Empty(PrimeFactorization.Distinct(value));

    [Fact]
    public void Distinct_IntMaxValue_YieldsItselfWithoutOverflow() =>
        Assert.Equal([int.MaxValue], PrimeFactorization.Distinct(int.MaxValue));
}
