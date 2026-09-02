using System.Numerics;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountWaysToMakeArrayWithProduct;

// LeetCode 1735. Count Ways to Make Array With Product: for each query (n, k),
// factor k into primes and, for every prime's exponent e, count the ways to split
// e identical "copies of that prime" across n ordered array slots - the classic
// stars-and-bars count C(e + n - 1, e) - then multiply those counts across primes
// (independent choices per prime). Factoring reuses this repo's own
// DynamicArray<int> as a smallest-prime-factor sieve, the same Sieve-of-Eratosthenes
// composite-marking pattern CountPrimesTests/PrimeArrangementsTests already
// establish with DynamicArray<bool>, generalized from a flag array to a factor
// array so every query after the sieve factors in O(log k) instead of O(sqrt(k)).
// The binomial coefficient itself is computed exactly via BigInteger (e is at most
// ~13 for k <= 10^4, so the running product/factorial never gets large) - the same
// "nothing to compose over plain integers" precedent SuperPowTests/
// PrimeArrangementsTests already accept for a handful of running scalars.
public sealed partial class CountWaysToMakeArrayWithProductTests
{
    private const int Mod = 1_000_000_007;

    [Fact]
    public void WaysToFillArray_LeetCodeExampleOne_ReturnsExpectedCounts()
    {
        int[][] queries = [[2, 6], [5, 1], [73, 660]];

        Assert.Equal([4, 1, 50_734_910], WaysToFillArrayWithProduct(queries));
    }

    [Fact]
    public void WaysToFillArray_LeetCodeExampleTwo_ReturnsExpectedCounts()
    {
        int[][] queries = [[1, 1], [2, 2], [3, 3], [4, 4], [5, 5]];

        Assert.Equal([1, 2, 3, 10, 5], WaysToFillArrayWithProduct(queries));
    }

    private static int[] WaysToFillArrayWithProduct(int[][] queries)
    {
        var maxK = queries.Max(q => q[1]);
        var smallestPrimeFactor = BuildSmallestPrimeFactorSieve(maxK);

        return queries.Select(q => WaysForQuery(q[0], q[1], smallestPrimeFactor)).ToArray();
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

    private static int WaysForQuery(int n, int k, DynamicArray<int> smallestPrimeFactor)
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

            result = result * BinomialMod(exponent + n - 1, exponent) % Mod;
        }

        return (int)result;
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
