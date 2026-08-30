using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Primes (LC 204): the O(n * sqrt(n)) trial-division brute force vs.
// the O(n log log n) Sieve of Eratosthenes, using this repo's own
// DynamicArray<bool> as the composite-tracking array.
[MemoryDiagnoser]
public class CountPrimesBenchmarks
{
    [Params(2_000, 20_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int TrialDivision()
    {
        var count = 0;

        for (var candidate = 2; candidate < N; candidate++)
        {
            var isPrime = true;

            for (var divisor = 2; divisor * divisor <= candidate; divisor++)
            {
                if (candidate % divisor == 0)
                {
                    isPrime = false;
                    break;
                }
            }

            if (isPrime)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int SieveOfEratosthenes()
    {
        if (N < 2)
        {
            return 0;
        }

        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i < N; i++)
        {
            isComposite.Add(false);
        }

        for (var i = 2; i * i < N; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple < N; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        var count = 0;
        for (var i = 2; i < N; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
