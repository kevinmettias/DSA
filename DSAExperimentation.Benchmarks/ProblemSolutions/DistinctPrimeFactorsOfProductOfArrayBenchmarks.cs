using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Distinct Prime Factors of Product of Array (LC 2521): forming the actual
// BigInteger product first (limb count grows every multiply, the same O(n^2)
// blowup shape AddTwoNumbersBenchmarks' BigInteger baseline hits) and then
// trial-dividing that huge number by every candidate up to the largest possible
// element value, vs. trial-dividing each (small, <= 1000) element on its own and
// deduping the results through this repo's own Set<int> - the product's prime
// factors are exactly the union of each element's, so the product itself never
// needs to be built.
[MemoryDiagnoser]
public class DistinctPrimeFactorsOfProductOfArrayBenchmarks
{
    private const int MaxValueInclusive = 1_000;
    private const int Seed = 2521; // LC problem number

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(2, MaxValueInclusive + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ProductThenTrialDivide()
    {
        var product = BuildProduct();
        var count = 0;

        for (var divisor = 2; divisor <= MaxValueInclusive; divisor++)
        {
            if (product % divisor != 0)
            {
                continue;
            }

            count++;

            while (product % divisor == 0)
            {
                product /= divisor;
            }
        }

        return count;
    }

    [Benchmark]
    public int PerElementFactorSet()
    {
        var primes = new Set<int>();

        foreach (var num in _values)
        {
            AddPrimeFactors(num, primes);
        }

        return primes.Count;
    }

    private BigInteger BuildProduct()
    {
        BigInteger product = 1;

        foreach (var num in _values)
        {
            product *= num;
        }

        return product;
    }

    private static void AddPrimeFactors(int value, Set<int> primes)
    {
        var remaining = value;

        for (var divisor = 2; divisor * divisor <= remaining; divisor++)
        {
            if (remaining % divisor != 0)
            {
                continue;
            }

            primes.TryAdd(divisor);

            while (remaining % divisor == 0)
            {
                remaining /= divisor;
            }
        }

        if (remaining > 1)
        {
            primes.TryAdd(remaining);
        }
    }
}
