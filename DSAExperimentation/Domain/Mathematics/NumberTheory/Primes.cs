namespace DSAExperimentation.Domain.Mathematics.NumberTheory;

// What every primality test, sieve and factorization agrees on. Testing, generating and factoring
// primes are procedures with interchangeable implementations of different cost, so they are
// Algorithms/NumberTheory's (Primality, PrimeSieve, PrimeFactorization); what holds whichever of
// them runs is stated here, and their tests hold them to it.
//
// 2 is the smallest prime and the only even one, so a search for primes begins there and a sieve's
// first crossing-out is of 2's multiples. 3 is the smallest odd prime and the only one 3 divides,
// so every prime above 3 is coprime to 2 · 3 = 6, and lies one either side of a multiple of 6:
// 6k - 1 or 6k + 1. The form is necessary, not sufficient (25 is 6 · 4 + 1), which is why a trial
// division may step by 6 and try two divisors a step. And every integer above 1 is a product of
// primes in exactly one way, up to their order, the fundamental theorem of arithmetic, so every
// factorization procedure must find the same primes the same number of times.
/// <summary>Facts about the prime numbers that every procedure over them agrees on.</summary>
public static class Primes
{
    /// <summary>The smallest prime, 2, and the only even one.</summary>
    public const int Smallest = 2;

    /// <summary>The smallest odd prime, 3, and the only prime 3 divides.</summary>
    public const int SmallestOdd = 3;

    // The wheel of the first two primes: every prime above them is coprime to their product, 6.
    private const int WheelOfTwoAndThree = Smallest * SmallestOdd;

    /// <summary>
    /// Whether a number is one either side of a multiple of 6, the form every prime above 3 has.
    /// </summary>
    /// <param name="value">The number, of either sign.</param>
    /// <returns>True when the number is 6k - 1 or 6k + 1 for some integer k.</returns>
    public static bool IsSixKPlusOrMinusOne(long value)
    {
        var remainder = Math.Abs(value % WheelOfTwoAndThree);
        return remainder == 1 || remainder == WheelOfTwoAndThree - 1;
    }
}
