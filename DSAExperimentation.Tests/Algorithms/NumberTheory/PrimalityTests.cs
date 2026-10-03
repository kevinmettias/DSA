using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class PrimalityTests
{
    private const int SieveBound = 1_000;

    public static TheoryData<int> ValuesBelowTwo => new() { -7, 0, 1 };

    [Theory]
    [MemberData(nameof(ValuesBelowTwo))]
    public void IsPrime_BelowTwo_IsFalse(int value) => Assert.False(Primality.IsPrime(value));

    // Trial division and the sieve are independent answers to the same question.
    [Fact]
    public void IsPrime_EveryValueUpToABound_AgreesWithTheSieve()
    {
        var isComposite = PrimeSieve.BuildCompositeTracker(SieveBound);

        for (var value = 0; value <= SieveBound; value++)
        {
            Assert.Equal(!isComposite[value], Primality.IsPrime(value));
        }
    }

    // int.MaxValue is the Mersenne prime 2^31 - 1; a divisor * divisor bound would overflow before
    // reaching its square root.
    [Fact]
    public void IsPrime_IntMaxValue_IsTrueWithoutOverflow() => Assert.True(Primality.IsPrime(int.MaxValue));

    [Fact]
    public void IsPrime_LongComposite_IsFalse() => Assert.False(Primality.IsPrime(1_000_000_007L * 3));
}
