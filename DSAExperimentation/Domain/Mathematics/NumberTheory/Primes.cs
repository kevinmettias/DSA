namespace DSAExperimentation.Domain.Mathematics.NumberTheory;

// What every sieve and every factorization starts from: 2 is the smallest prime, and the only even
// one, so a search for primes begins there and a sieve's first crossing-out is of 2's multiples.
/// <summary>Facts about the prime numbers.</summary>
public static class Primes
{
    /// <summary>The smallest prime, 2, and the only even one.</summary>
    public const int Smallest = 2;
}

// TODO: THIS CLASS SEEMS TO SPARCE, CAN WE ADD MORE UTILITIES LIKE PRIME CHECKING, GENERATING PRIMES, OR FACTORIZATION METHODS?