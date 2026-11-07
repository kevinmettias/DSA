using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.NumberTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.NumberTheory;

// The facts every procedure over the primes agrees on, each held to Algorithms/NumberTheory's
// procedures over a range: trial division (Primality, PrimeFactorization) and the sieve
// (PrimeSieve).
public sealed partial class PrimesTests
{
    private const int Bound = 20_000;

    private static readonly Lazy<bool[]> IsComposite = new(() => PrimeSieve.BuildCompositeTracker(Bound));
    private static readonly Lazy<int[]> SmallestFactor = new(() => PrimeSieve.BuildSmallestPrimeFactors(Bound));

    [Fact]
    public void Smallest_IsPrime() => Assert.True(Primality.IsPrime(Primes.Smallest));

    [Fact]
    public void Smallest_HasNoPrimeBelowIt() => Assert.DoesNotContain(Enumerable.Range(0, Primes.Smallest), Primality.IsPrime);

    [Fact]
    public void Smallest_IsTheOnlyEvenPrimeToTheBound() =>
        Assert.All(
            Enumerable.Range(0, Bound + 1).Where(value => value % 2 == 0),
            value =>
            {
                Assert.Equal(value == Primes.Smallest, Primality.IsPrime(value));
                Assert.Equal(value == Primes.Smallest, !IsComposite.Value[value]);
            });

    [Fact]
    public void SmallestOdd_IsPrimeAndNoOddNumberBelowItIs()
    {
        Assert.True(Primality.IsPrime(Primes.SmallestOdd));
        Assert.DoesNotContain(Enumerable.Range(0, Primes.SmallestOdd).Where(value => value % 2 == 1), Primality.IsPrime);
    }

    [Fact]
    public void IsSixKPlusOrMinusOne_EveryPrimeAboveThree_IsTrue() =>
        Assert.All(
            Enumerable.Range(Primes.SmallestOdd + 1, Bound - Primes.SmallestOdd).Where(value => !IsComposite.Value[value]),
            prime => Assert.True(Primes.IsSixKPlusOrMinusOne(prime)));

    // The form is exactly being coprime to 6, for numbers of either sign.
    [Fact]
    public void IsSixKPlusOrMinusOne_IsBeingCoprimeToSix() =>
        Assert.All(
            Enumerable.Range(-100, 201),
            value => Assert.Equal(GreatestCommonDivisor.Of(Math.Abs(value), 6) == 1, Primes.IsSixKPlusOrMinusOne(value)));

    [Fact]
    public void IsSixKPlusOrMinusOne_IsNotSufficient() =>
        Assert.True(Primes.IsSixKPlusOrMinusOne(25) && !Primality.IsPrime(25));

    // The fundamental theorem of arithmetic as the contract the factorizations meet: the sieve's
    // smallest factors and trial division find the same primes, each the same number of times, and
    // their product is the number.
    [Fact]
    public void Factorizations_EveryNumberToTheBound_AreOneAndTheSame() =>
        Assert.All(
            Enumerable.Range(Primes.Smallest, Bound - 1),
            value =>
            {
                var bySieve = FactorsBySieve(value);
                Assert.Equal(bySieve, FactorsByTrialDivision(value));
                Assert.Equal(value, bySieve.Aggregate(1L, (product, factor) => product * factor));
                Assert.All(bySieve, factor => Assert.True(Primality.IsPrime(factor) && !IsComposite.Value[factor]));
            });

    private static List<int> FactorsBySieve(int value)
    {
        var factors = new List<int>();

        for (var remaining = value; remaining > 1; remaining /= SmallestFactor.Value[remaining])
        {
            factors.Add(SmallestFactor.Value[remaining]);
        }

        return factors;
    }

    private static List<int> FactorsByTrialDivision(int value)
    {
        var factors = new List<int>();

        foreach (var prime in PrimeFactorization.Distinct(value))
        {
            for (var remaining = value; remaining % prime == 0; remaining /= prime)
            {
                factors.Add(prime);
            }
        }

        return factors;
    }
}
