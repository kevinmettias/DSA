using DSAExperimentation.Algorithms.NumberTheory;
using System.Numerics;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToMakeArrayWithProduct;

// LeetCode 1735. Count Ways to Make Array With Product: for each query (n, k),
// count the length-n positive-integer arrays whose product is exactly k, modulo
// 1e9+7.
//
// Both strategies answer it the same way and differ only in how they factor the
// target product. Factor that product into primes and, for every prime's exponent
// e, count the ways to split e identical "copies of that prime" across arrayLength
// ordered slots - the classic stars-and-bars count C(e + n - 1, e) - then multiply
// those counts across primes, because each prime is distributed independently. The
// binomial coefficient itself is computed exactly via BigInteger (e is at most ~13
// for k <= 10^4, so the running product never grows), the same "nothing to compose
// over a handful of running scalars" precedent SuperPow and PrimeArrangements
// already accept.
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

    // The textbook answer: factor every query's target product from scratch by trial
    // division, sharing nothing between queries. Deliberately written over BCL arrays
    // only - it is the arm the sieve below has to justify itself against.
    public static int[] WaysToFillArrayByTrialDivision(int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = WaysByTrialDivision(queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static int WaysByTrialDivision(int arrayLength, int targetProduct)
    {
        var remaining = targetProduct;
        var result = 1L;

        for (var factor = SmallestPrime; (long)factor * factor <= remaining; factor++)
        {
            result = ApplyTrialDivisionFactor(factor, arrayLength, ref remaining, result);
        }

        if (remaining > 1)
        {
            result = result * BinomialMod(arrayLength, LeftoverPrimeExponent) % ModularArithmetic.Modulo;
        }

        return (int)result;
    }

    private static long ApplyTrialDivisionFactor(int factor, int arrayLength, ref int remaining, long result)
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

        return result * BinomialMod(exponent + arrayLength - 1, exponent) % ModularArithmetic.Modulo;
    }

    // Build one smallest-prime-factor table over the largest product in the batch with
    // this repo's own PrimeSieve - the factor-array form of the composite-marking sieve -
    // so every query afterwards factors in O(log k).
    public static int[] WaysToFillArrayBySmallestPrimeFactorSieve(int[][] queries)
    {
        var maxK = queries.Max(query => query[1]);
        var smallestPrimeFactor = PrimeSieve.BuildSmallestPrimeFactors(maxK);
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = WaysBySieve(queries[i][0], queries[i][1], smallestPrimeFactor);
        }

        return answers;
    }

    private static int WaysBySieve(int arrayLength, int targetProduct, int[] smallestPrimeFactor)
    {
        var remaining = targetProduct;
        var result = 1L;

        while (remaining > 1)
        {
            var factor = smallestPrimeFactor[remaining];
            var exponent = 0;

            while (remaining % factor == 0)
            {
                remaining /= factor;
                exponent++;
            }

            result = result * BinomialMod(exponent + arrayLength - 1, exponent) % ModularArithmetic.Modulo;
        }

        return (int)result;
    }

    private static long BinomialMod(int total, int chooseCount)
    {
        BigInteger numerator = 1;
        for (var i = 0; i < chooseCount; i++)
        {
            numerator *= total - i;
        }

        BigInteger denominator = 1;
        for (var i = FactorialStartMultiplier; i <= chooseCount; i++)
        {
            denominator *= i;
        }

        return (long)(numerator / denominator % ModularArithmetic.Modulo);
    }
}
