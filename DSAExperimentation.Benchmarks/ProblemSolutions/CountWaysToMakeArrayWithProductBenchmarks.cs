using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Ways to Make Array With Product (LC 1735): factoring every query's k from
// scratch by trial division vs. building this repo's own DynamicArray<int>
// smallest-prime-factor sieve once (same Sieve-of-Eratosthenes pattern
// CountPrimesBenchmarks/PrimeArrangementsBenchmarks already establish with
// DynamicArray<bool>) and reusing it across every query for O(log k) factoring.
// Trial division re-pays O(sqrt(k)) on every single query; the shared sieve pays
// O(maxK log log maxK) once and amortizes it across QueryCount queries.
[MemoryDiagnoser]
public class CountWaysToMakeArrayWithProductBenchmarks
{
    private const int Mod = 1_000_000_007;
    private const int MaxK = 10_000;

    // Seeds query generation; not the LC problem number here.
    private const int RandomSeed = 6;

    // Exclusive upper bound for the generated array length n in each query.
    private const int MaxArrayLength = 50;

    // Smallest prime, and the starting point for both trial division and the sieve.
    private const int SmallestPrime = 2;

    // 1! == 1, so factorial products only need to start accumulating from 2.
    private const int FactorialStartMultiplier = 2;

    [Params(200, 2_000)]
    public int QueryCount;

    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(1, MaxArrayLength), random.Next(1, MaxK + 1) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long TrialDivisionPerQuery()
    {
        var total = 0L;
        foreach (var query in _queries)
        {
            total += WaysForQueryTrialDivision(query[0], query[1]);
        }

        return total;
    }

    [Benchmark]
    public long SmallestPrimeFactorSieve()
    {
        var spf = BuildSmallestPrimeFactorSieve(MaxK);
        var total = 0L;
        foreach (var query in _queries)
        {
            total += WaysForQuery(query[0], query[1], spf);
        }

        return total;
    }

    private static int WaysForQueryTrialDivision(int n, int k)
    {
        var remaining = k;
        var result = 1L;

        for (var factor = SmallestPrime; (long)factor * factor <= remaining; factor++)
        {
            result = ApplyTrialDivisionFactor(factor, n, ref remaining, result);
        }

        if (remaining > 1)
        {
            result = result * BinomialMod(n, 1) % Mod;
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

        return result * BinomialMod(exponent + n - 1, exponent) % Mod;
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
        for (var i = FactorialStartMultiplier; i <= r; i++)
        {
            denominator *= i;
        }

        return (long)(numerator / denominator % Mod);
    }
}
