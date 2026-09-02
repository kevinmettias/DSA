using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Closest Prime Numbers in Range (LC 2523): trial-dividing every candidate in
// [2, right] up to sqrt(candidate) - O((right) * sqrt(right)) - vs. the same Sieve
// of Eratosthenes CountPrimesBenchmarks already runs over this repo's own
// DynamicArray<bool> composite tracker - O(right log log right) - with the sieved
// range scanned once afterward for the closest pair either way.
[MemoryDiagnoser]
public class ClosestPrimeNumbersInRangeBenchmarks
{
    private const int Left = 2;

    [Params(2_000, 20_000)]
    public int Right;

    [Benchmark(Baseline = true)]
    public int TrialDivisionScan()
    {
        var previousPrime = -1;
        var bestGap = int.MaxValue;

        for (var candidate = Left; candidate <= Right; candidate++)
        {
            if (!IsPrime(candidate))
            {
                continue;
            }

            if (previousPrime != -1)
            {
                bestGap = Math.Min(bestGap, candidate - previousPrime);
            }

            previousPrime = candidate;
        }

        return bestGap;
    }

    [Benchmark]
    public int SieveScan()
    {
        var isComposite = BuildSieve();
        var previousPrime = -1;
        var bestGap = int.MaxValue;

        for (var candidate = Left; candidate <= Right; candidate++)
        {
            if (isComposite.Get(candidate))
            {
                continue;
            }

            if (previousPrime != -1)
            {
                bestGap = Math.Min(bestGap, candidate - previousPrime);
            }

            previousPrime = candidate;
        }

        return bestGap;
    }

    private DynamicArray<bool> BuildSieve()
    {
        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i <= Right; i++)
        {
            isComposite.Add(i < 2);
        }

        for (var i = 2; (long)i * i <= Right; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= Right; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }

    private static bool IsPrime(int value)
    {
        if (value < 2)
        {
            return false;
        }

        if (value % 2 == 0)
        {
            return value == 2;
        }

        for (var divisor = 3; (long)divisor * divisor <= value; divisor += 2)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }
}
