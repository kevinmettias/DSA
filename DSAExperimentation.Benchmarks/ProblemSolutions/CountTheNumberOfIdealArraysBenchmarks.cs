using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count the Number of Ideal Arrays (LC 2338): factoring every value from 1 to maxValue by trial
// division vs. building this repo's own DynamicArray<int> smallest-prime-factor sieve once (the
// exact CountWaysToMakeArrayWithProductBenchmarks (LC 1735) contrast, reshaped from "per query" to
// "per value 1..maxValue" since this problem sums over every value itself rather than answering a
// separate query list) and reusing it for O(log value) factoring of every value instead of paying
// O(sqrt(value)) per value from scratch. Both benchmarks compute the exact same sum-over-primes
// binomial product CountTheNumberOfIdealArraysTests uses, for a fixed array length N.
[MemoryDiagnoser]
public class CountTheNumberOfIdealArraysBenchmarks
{
    private const int Mod = 1_000_000_007;
    private const int SmallestPrime = 2;
    private const int FactorialStartMultiplier = 2;
    private const int N = 4;

    [Params(200, 5_000)]
    public int MaxValue;

    [Benchmark(Baseline = true)]
    public long TrialDivisionPerValue()
    {
        var total = 0L;

        for (var value = 1; value <= MaxValue; value++)
        {
            total = (total + CountArraysEndingAtTrialDivision(value, N)) % Mod;
        }

        return total;
    }

    [Benchmark]
    public long SmallestPrimeFactorSieve()
    {
        var spf = BuildSmallestPrimeFactorSieve(MaxValue);
        var total = 0L;

        for (var value = 1; value <= MaxValue; value++)
        {
            total = (total + CountArraysEndingAt(value, N, spf)) % Mod;
        }

        return total;
    }

    private static long CountArraysEndingAtTrialDivision(int value, int n)
    {
        var remaining = value;
        var product = 1L;

        for (var factor = SmallestPrime; (long)factor * factor <= remaining; factor++)
        {
            if (remaining % factor != 0)
            {
                continue;
            }

            var exponent = 0;
            while (remaining % factor == 0)
            {
                remaining /= factor;
                exponent++;
            }

            product = product * BinomialMod(exponent + n - 1, exponent) % Mod;
        }

        if (remaining > 1)
        {
            product = product * BinomialMod(n, 1) % Mod;
        }

        return product;
    }

    private static long CountArraysEndingAt(int value, int n, DynamicArray<int> smallestPrimeFactor)
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

            product = product * BinomialMod(exponent + n - 1, exponent) % Mod;
        }

        return product;
    }

    private static DynamicArray<int> BuildSmallestPrimeFactorSieve(int max)
    {
        var spf = CreateIdentityArray(max);
        PopulateSmallestPrimeFactors(spf, max);
        return spf;
    }

    private static DynamicArray<int> CreateIdentityArray(int max)
    {
        var spf = new DynamicArray<int>();
        for (var i = 0; i <= max; i++)
        {
            spf.Add(i);
        }

        return spf;
    }

    private static void PopulateSmallestPrimeFactors(DynamicArray<int> spf, int max)
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

        return (long)(numerator / denominator % Mod);
    }
}
