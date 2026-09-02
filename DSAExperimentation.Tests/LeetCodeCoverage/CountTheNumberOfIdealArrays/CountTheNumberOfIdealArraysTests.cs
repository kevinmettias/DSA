using System.Numerics;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfIdealArrays;

// LeetCode 2338. Count the Number of Ideal Arrays: arr[i] | arr[i+1] forces arr to be
// non-decreasing, so every ideal array is pinned down by its LAST value V (<= maxValue) plus,
// independently per prime factor of V, a non-decreasing sequence of exponents across the n array
// slots that ends at that prime's exponent in V - the classic stars-and-bars count
// C(n-1+exponent, exponent) per prime, multiplied across V's distinct primes and summed over
// every V from 1 to maxValue (hand-verified below against both of LeetCode's own published
// examples). Factoring each V reuses this repo's own DynamicArray<int> as a smallest-prime-factor
// sieve - the exact same Sieve-of-Eratosthenes-style composite-marking
// CountWaysToMakeArrayWithProductTests (LC 1735) already establishes for this same "factor every
// value up to a bound, once, via a shared sieve" role - and each binomial coefficient reuses that
// same test's BigInteger-exact-division BinomialMod (the exponent is at most ~13 for
// maxValue <= 10^4, so the running product/factorial never gets large enough to need a
// modular-inverse primitive this repo doesn't have).
public sealed partial class CountTheNumberOfIdealArraysTests
{
    private const int Mod = 1_000_000_007;

    [Fact]
    public void CountIdealArrays_LeetCodeExampleOne_ReturnsTen()
    {
        Assert.Equal(10, CountIdealArrays(n: 2, maxValue: 5));
    }

    [Fact]
    public void CountIdealArrays_LeetCodeExampleTwo_ReturnsEleven()
    {
        Assert.Equal(11, CountIdealArrays(n: 5, maxValue: 3));
    }

    [Fact]
    public void CountIdealArrays_LengthOne_EveryValueIsItsOwnIdealArray()
    {
        Assert.Equal(10, CountIdealArrays(n: 1, maxValue: 10));
    }

    private static int CountIdealArrays(int n, int maxValue)
    {
        var smallestPrimeFactor = BuildSmallestPrimeFactorSieve(maxValue);
        var total = 0L;

        for (var value = 1; value <= maxValue; value++)
        {
            total = (total + CountArraysEndingAt(value, n, smallestPrimeFactor)) % Mod;
        }

        return (int)total;
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
        for (var i = 2; (long)i * i <= max; i++)
        {
            if (spf.Get(i) != i)
            {
                continue;
            }

            for (var multiple = i * i; multiple <= max; multiple += i)
            {
                if (spf.Get(multiple) == multiple)
                {
                    spf.Set(multiple, i);
                }
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
        for (var i = 2; i <= r; i++)
        {
            denominator *= i;
        }

        return (long)(numerator / denominator % Mod);
    }
}
