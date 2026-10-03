using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class PrimeSieveTests
{
    private const int SmallBound = 30;

    private static readonly int[] PrimesUpToSmallBound = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29];

    [Fact]
    public void BuildCompositeTracker_SmallBound_LeavesExactlyThePrimesClear()
    {
        var isComposite = PrimeSieve.BuildCompositeTracker(SmallBound);

        Assert.Equal(PrimesUpToSmallBound, Enumerable.Range(0, isComposite.Length).Where(value => !isComposite[value]));
    }

    // The range is inclusive: a prime bound is itself decided.
    [Fact]
    public void BuildCompositeTracker_PrimeBound_IncludesTheBound() =>
        Assert.False(PrimeSieve.BuildCompositeTracker(29)[29]);

    [Fact]
    public void BuildCompositeTracker_ZeroAndOne_CountAsComposite() =>
        Assert.Equal([true, true], PrimeSieve.BuildCompositeTracker(1));

    [Fact]
    public void BuildCompositeTracker_NegativeBound_IsAnEmptyRange() =>
        Assert.Empty(PrimeSieve.BuildCompositeTracker(-1));

    [Fact]
    public void BuildSmallestPrimeFactors_SmallBound_MatchesTrialDivision()
    {
        var smallestFactor = PrimeSieve.BuildSmallestPrimeFactors(SmallBound);

        for (var value = 2; value <= SmallBound; value++)
        {
            Assert.Equal(PrimeFactorization.Distinct(value).First(), smallestFactor[value]);
        }
    }

    [Fact]
    public void BuildSmallestPrimeFactors_ZeroAndOne_HoldThemselves() =>
        Assert.Equal([0, 1], PrimeSieve.BuildSmallestPrimeFactors(1));
}
