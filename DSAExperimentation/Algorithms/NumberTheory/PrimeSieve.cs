using DSAExperimentation.Domain.Mathematics.NumberTheory;
namespace DSAExperimentation.Algorithms.NumberTheory;

// The sieve of Eratosthenes over a value bound, in two forms: which values in [0, bound] are
// composite, and each value's smallest prime factor. Both decide every value up to the bound at once
// in O(bound log log bound), where trial division decides one value per call.
//
// The range is inclusive of `bound`, and a negative bound is an empty range rather than an error.
// A caller counting primes strictly below n passes n - 1.
//
// Multiples are stepped as long: a bound near int.MaxValue would otherwise overflow both the
// starting square and the step, and the overflow would break the loop's exit test rather than
// the sieve itself.
/// <summary>The sieve of Eratosthenes, deciding every value in [0, bound] at once.</summary>
public static class PrimeSieve
{

    // Slot i is true when i is composite. 0 and 1 count as composite, so "is the slot clear" is the
    // whole primality test, 0 and 1 included.
    /// <summary>Marks which values from 0 through a bound are not prime.</summary>
    /// <param name="bound">The largest value to decide; a negative bound is an empty range.</param>
    /// <returns>
    /// An array of bound + 1 slots, true where the index is composite or is 0 or 1, so a clear slot means
    /// prime; empty for a negative bound.
    /// </returns>
    public static bool[] BuildCompositeTracker(int bound)
    {
        var isComposite = new bool[SlotCount(bound)];

        for (var value = 0; value < Math.Min(Primes.Smallest, isComposite.Length); value++)
        {
            isComposite[value] = true;
        }

        for (var prime = Primes.Smallest; (long)prime * prime <= bound; prime++)
        {
            if (!isComposite[prime])
            {
                for (var multiple = (long)prime * prime; multiple <= bound; multiple += prime)
                {
                    isComposite[multiple] = true;
                }
            }
        }

        return isComposite;
    }

    // Slot i holds i's smallest prime factor; a prime is its own, and 0 and 1 hold themselves.
    // Repeatedly dividing a value by its slot factors it completely in O(log value) steps.
    /// <summary>Finds the smallest prime factor of every value from 0 through a bound.</summary>
    /// <param name="bound">The largest value to factor; a negative bound is an empty range.</param>
    /// <returns>
    /// An array of bound + 1 slots, slot i holding i's smallest prime factor, with a prime, 0 and 1 each
    /// holding itself; empty for a negative bound.
    /// </returns>
    public static int[] BuildSmallestPrimeFactors(int bound)
    {
        var smallestFactor = new int[SlotCount(bound)];

        for (var value = 0; value < smallestFactor.Length; value++)
        {
            smallestFactor[value] = value;
        }

        for (var prime = Primes.Smallest; (long)prime * prime <= bound; prime++)
        {
            if (smallestFactor[prime] == prime)
            {
                for (var multiple = (long)prime * prime; multiple <= bound; multiple += prime)
                {
                    smallestFactor[multiple] = Math.Min(smallestFactor[multiple], prime);
                }
            }
        }

        return smallestFactor;
    }

    private static int SlotCount(int bound) => Math.Max(bound + 1, 0);
}
