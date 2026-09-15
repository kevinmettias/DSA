using System.Numerics;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfIdealArrays;

// LeetCode 2338. Count the Number of Ideal Arrays: count the length-n arrays whose
// every value is in [1, maxValue] and where each element divides the next, modulo
// 1e9+7.
//
// arr[i] | arr[i+1] forces arr to be non-decreasing, so an ideal array is pinned down
// by its LAST value V (<= maxValue) plus, independently per prime factor of V, a
// non-decreasing sequence of exponents across the n slots ending at that prime's
// exponent in V - the classic stars-and-bars count C(n - 1 + e, e) per prime,
// multiplied across V's distinct primes and summed over every V from 1 to maxValue.
// That is the same per-prime stars-and-bars product
// CountWaysToMakeArrayWithProductSolution computes for LC 1735, reshaped from "per
// query k" to "summed over every value 1..maxValue", and the binomial coefficient is
// computed exactly the same way: BigInteger exact division, since e is at most ~13 for
// maxValue <= 10^4 so the running product never grows enough to need a
// modular-inverse primitive.
//
// What separates the arms is the cost of factoring: trial division re-pays
// O(sqrt(value)) for every one of the maxValue values, while one smallest-prime-factor
// sieve pays O(maxValue log log maxValue) once and then factors each value in
// O(log value).
internal static class CountTheNumberOfIdealArraysSolution
{
    // Smallest prime, and the starting point for both trial division and the sieve.
    private const int SmallestPrime = 2;

    // 1! == 1, so a factorial product only needs to start accumulating from 2.
    private const int FactorialStartMultiplier = 2;

    // A prime factor larger than sqrt(value) can only survive to exponent one.
    private const int LeftoverPrimeExponent = 1;

    // The textbook answer: factor each value from scratch by trial division, sharing
    // nothing between values. Deliberately written over BCL scalars only - it is the
    // arm the sieve below has to justify itself against.
    public static int IdealArraysByTrialDivision(int n, int maxValue)
    {
        var total = 0L;

        for (var value = 1; value <= maxValue; value++)
        {
            total = (total + CountEndingAtByTrialDivision(value, n)) % ModularArithmetic.Modulo;
        }

        return (int)total;
    }

    private static long CountEndingAtByTrialDivision(int value, int n)
    {
        var remaining = value;
        var product = 1L;

        for (var factor = SmallestPrime; (long)factor * factor <= remaining; factor++)
        {
            product = ApplyTrialDivisionFactor(factor, n, ref remaining, product);
        }

        if (remaining > 1)
        {
            product = product * BinomialMod(n, LeftoverPrimeExponent) % ModularArithmetic.Modulo;
        }

        return product;
    }

    private static long ApplyTrialDivisionFactor(int factor, int n, ref int remaining, long product)
    {
        if (remaining % factor != 0)
        {
            return product;
        }

        var exponent = 0;
        while (remaining % factor == 0)
        {
            remaining /= factor;
            exponent++;
        }

        return product * BinomialMod(exponent + n - 1, exponent) % ModularArithmetic.Modulo;
    }

    // Build one smallest-prime-factor table over maxValue, using this repo's own
    // DynamicArray<int> as the sieve buffer - the same Sieve-of-Eratosthenes
    // composite-marking pattern CountWaysToMakeArrayWithProduct establishes for LC
    // 1735 - so every value afterwards factors in O(log value).
    public static int IdealArraysBySmallestPrimeFactorSieve(int n, int maxValue)
    {
        var smallestPrimeFactor = BuildSmallestPrimeFactorSieve(maxValue);
        var total = 0L;

        for (var value = 1; value <= maxValue; value++)
        {
            total = (total + CountEndingAtBySieve(value, n, smallestPrimeFactor)) % ModularArithmetic.Modulo;
        }

        return (int)total;
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

    private static long CountEndingAtBySieve(int value, int n, DynamicArray<int> smallestPrimeFactor)
    {
        var remaining = value;
        var product = 1L;

        while (remaining > 1)
        {
            var factor = smallestPrimeFactor.Get(remaining);
            var exponent = 0;

            while (remaining % factor == 0)
            {
                remaining /= factor;
                exponent++;
            }

            product = product * BinomialMod(exponent + n - 1, exponent) % ModularArithmetic.Modulo;
        }

        return product;
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
