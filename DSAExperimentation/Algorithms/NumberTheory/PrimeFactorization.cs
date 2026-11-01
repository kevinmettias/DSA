using System.Numerics;

namespace DSAExperimentation.Algorithms.NumberTheory;

// The distinct prime factors of one value by trial division, ascending, each once however often it
// divides: 12 yields 2 then 3. Lazy, so a caller that stops at the first factor pays only for it.
// Values below 2 have no prime factors and yield nothing.
//
// The loop bound is factor <= remaining / factor, which cannot overflow Integer, rather than
// factor * factor <= remaining.
/// <summary>The distinct prime factors of one value, found by trial division.</summary>
public static class PrimeFactorization
{
    /// <summary>Lists the primes that divide a value, in ascending order and each once.</summary>
    /// <param name="value">The value to factor, of any binary integer type.</param>
    /// <returns>
    /// The prime factors, computed lazily as they are read, so 12 yields 2 then 3; empty for a value below 2.
    /// </returns>
    public static IEnumerable<Integer> Distinct<Integer>(Integer value)
        where Integer : IBinaryInteger<Integer>
    {
        var two = Integer.One + Integer.One;
        var remaining = value;

        for (var factor = two; factor <= remaining / factor; factor++)
        {
            if (remaining % factor != Integer.Zero)
            {
                continue;
            }

            yield return factor;

            while (remaining % factor == Integer.Zero)
            {
                remaining /= factor;
            }
        }

        if (remaining >= two)
        {
            yield return remaining;
        }
    }
}
