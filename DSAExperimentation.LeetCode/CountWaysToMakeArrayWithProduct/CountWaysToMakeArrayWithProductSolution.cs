using System.Numerics;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToMakeArrayWithProduct;

// LeetCode 1735. Count Ways to Make Array With Product: for each query (n, k),
// count the length-n positive-integer arrays whose product is exactly k, modulo
// 1e9+7.
//
// Both strategies answer it the same way and differ only in how they factor k.
// Factor k into primes and, for every prime's exponent e, count the ways to split
// e identical "copies of that prime" across n ordered slots - the classic
// stars-and-bars count C(e + n - 1, e) - then multiply those counts across primes,
// because each prime is distributed independently. The binomial coefficient itself
// is computed exactly via BigInteger (e is at most ~13 for k <= 10^4, so the
// running product never grows), the same "nothing to compose over a handful of
// running scalars" precedent SuperPow and PrimeArrangements already accept.
//
// What separates the arms is the cost of factoring: trial division re-pays
// O(sqrt(k)) on every single query, while one shared smallest-prime-factor sieve
// pays O(maxK log log maxK) once and then factors each k in O(log k).
internal static class CountWaysToMakeArrayWithProductSolution
{
    // Smallest prime, and the starting point for both trial division and the sieve.
    private const int SmallestPrime = 2;

    // 1! == 1, so a factorial product only needs to start accumulating from 2.
    private const int FactorialStartMultiplier = 2;

    // A prime factor larger than sqrt(k) can only survive to exponent one.
    private const int LeftoverPrimeExponent = 1;

    // The textbook answer: factor every query's k from scratch by trial division,
    // sharing nothing between queries. Deliberately written over BCL arrays only -
    // it is the arm the sieve below has to justify itself against.
    public static int[] WaysToFillArrayByTrialDivision(int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = WaysByTrialDivision(queries[i][0], queries[i][1]);
        }

        return answers;
    }

    // Build one smallest-prime-factor table over the largest k in the batch, using
    // this repo's own DynamicArray<int> as the sieve buffer - the same
    // Sieve-of-Eratosthenes composite-marking pattern CountPrimes and
    // PrimeArrangements establish with DynamicArray<bool>, generalized from a flag
    // array to a factor array so every query afterwards factors in O(log k).
    public static int[] WaysToFillArrayBySmallestPrimeFactorSieve(int[][] queries)
    {
        var maxK = queries.Max(query => query[1]);
        var smallestPrimeFactor = BuildSmallestPrimeFactorSieve(maxK);
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = WaysBySieve(queries[i][0], queries[i][1], smallestPrimeFactor);
        }

        return answers;
    }

    private static int WaysByTrialDivision(int n, int k)
    {
        var remaining = k;
        var result = 1L;

        for (var factor = SmallestPrime; (long)factor * factor <= remaining; factor++)
        {
            result = ApplyTrialDivisionFactor(factor, n, ref remaining, result);
        }

        if (remaining > 1)
        {
            result = result * BinomialMod(n, LeftoverPrimeExponent) % ModularArithmetic.Modulo;
        }

        return (int)result;
    }

    private static long ApplyTrialDivisionFactor(int factor, int n, ref int remaining, long result)
    {
        if (remaining % factor != 0)
        {
            return result;
        }

        var exponent = 0;
        while (remaining % factor == 0)
        {
            remaining /= factor;
            exponent++;
        }

        return result * BinomialMod(exponent + n - 1, exponent) % ModularArithmetic.Modulo;
    }

    private static int WaysBySieve(int n, int k, DynamicArray<int> smallestPrimeFactor)
    {
        var remaining = k;
        var result = 1L;

        while (remaining > 1)
        {
            var factor = smallestPrimeFactor.Get(remaining);
            var exponent = 0;

            while (remaining % factor == 0)
            {
                remaining /= factor;
                exponent++;
            }

            result = result * BinomialMod(exponent + n - 1, exponent) % ModularArithmetic.Modulo;
        }

        return (int)result;
    }

    // spf[i] holds i's smallest prime factor (spf[i] == i means i is prime, or 1).
    private static DynamicArray<int> BuildSmallestPrimeFactorSieve(int max)
    {
        var spf = InitializeIdentitySieve(max);
        MarkSmallestPrimeFactors(spf, max);
        return spf;
    }

    private static DynamicArray<int> InitializeIdentitySieve(int max)
    {
        var spf = new DynamicArray<int>();

        for (var i = 0; i <= max; i++)
        {
            spf.Add(i);
        }

        return spf;
    }

    private static void MarkSmallestPrimeFactors(DynamicArray<int> spf, int max)
    {
        for (var i = SmallestPrime; (long)i * i <= max; i++)
        {
            if (spf.Get(i) != i)
            {
                continue;
            }

            MarkMultiples(spf, i, max);
        }
    }

    private static void MarkMultiples(DynamicArray<int> spf, int prime, int max)
    {
        for (var multiple = prime * prime; multiple <= max; multiple += prime)
        {
            if (spf.Get(multiple) == multiple)
            {
                spf.Set(multiple, prime);
            }
        }
    }

    private static long BinomialMod(int total, int r)
    {
        BigInteger numerator = 1;
        for (var i = 0; i < r; i++)
        {
            numerator *= total - i;
        }

        BigInteger denominator = 1;
        for (var i = FactorialStartMultiplier; i <= r; i++)
        {
            denominator *= i;
        }

        return (long)(numerator / denominator % ModularArithmetic.Modulo);
    }
}
