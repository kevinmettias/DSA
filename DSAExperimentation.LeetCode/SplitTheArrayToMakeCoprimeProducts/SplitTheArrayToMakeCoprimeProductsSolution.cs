using System.Numerics;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.SplitTheArrayToMakeCoprimeProducts;

// LeetCode 2584. Split the Array to Make Coprime Products: the smallest index i
// such that the product of nums[0..i] and the product of nums[i+1..n-1] share no
// common factor, or -1 when no such split exists.
//
// The two strategies are the two readings of "coprime". Take the definition
// literally - form both products and ask for their gcd at every candidate split -
// or notice that the two sides share a factor exactly when some prime divides an
// element on each side, which turns the whole question into "where does the last
// element carrying a prime seen so far sit?" and never multiplies anything.
internal static class SplitTheArrayToMakeCoprimeProductsSolution
{
    private const int SmallestPrime = 2;

    // The textbook answer: a running BigInteger left product and the complementary
    // right product, taking a gcd at every candidate split. Deliberately BCL-only,
    // and deliberately paying for numbers that outgrow long within a handful of
    // elements - it is the arm the prime-boundary sweep below has to justify
    // itself against.
    public static int FindValidSplitByProductGcd(int[] nums)
    {
        var rightProduct = BigInteger.One;

        foreach (var value in nums)
        {
            rightProduct *= value;
        }

        var leftProduct = BigInteger.One;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            leftProduct *= nums[i];
            rightProduct /= nums[i];

            if (BigInteger.GreatestCommonDivisor(leftProduct, rightProduct) == BigInteger.One)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Composed: factor every element once into this repo's own HashMap<prime,
    // lastIndex> recording each prime's rightmost occurrence - the same "map a key
    // to the latest index that touches it" shape LargestComponentSizeByCommonFactor
    // uses for shared prime factors, tracking an index rather than a component
    // owner. A split at i is coprime exactly when no prime appearing in nums[0..i]
    // ever reappears past i, so one left-to-right sweep keeping the running
    // boundary (the largest last-occurrence among primes seen so far) answers the
    // problem the moment boundary == i. No products are ever formed.
    public static int FindValidSplitByPrimeLastOccurrence(int[] nums)
    {
        var lastOccurrence = LastOccurrenceByPrime(nums);
        var boundary = 0;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.TryGetValue(factor, out var last);
                boundary = Math.Max(boundary, last);
            }

            if (boundary == i)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Later writes overwrite earlier ones, so a left-to-right pass leaves each
    // prime mapped to the rightmost element it divides.
    private static HashMap<int, int> LastOccurrenceByPrime(int[] nums)
    {
        var lastOccurrence = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.Set(factor, i);
            }
        }

        return lastOccurrence;
    }

    // Trial division up to the square root, yielding each distinct prime once; a
    // remainder above 1 after the loop is itself the last prime factor.
    private static IEnumerable<int> PrimeFactors(int value)
    {
        var remaining = value;

        for (var factor = SmallestPrime; factor * factor <= remaining; factor++)
        {
            if (remaining % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (remaining % factor == 0)
            {
                remaining /= factor;
            }
        }

        if (remaining > 1)
        {
            yield return remaining;
        }
    }
}
