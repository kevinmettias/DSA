using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prime Arrangements (LC 1175): both strategies compute the same
// primeCount! * compositeCount! mod 1e9+7 answer; they differ only in how the
// prime count is produced - O(n * sqrt(n)) trial division vs. the O(n log log n)
// Sieve of Eratosthenes over this repo's own DynamicArray<bool>, the same
// composite-tracking array CountPrimesBenchmarks uses.
[MemoryDiagnoser]
public class PrimeArrangementsBenchmarks
{
    private const int Modulo = 1_000_000_007;

    [Params(2_000, 20_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int TrialDivision() => NumPrimeArrangements(CountPrimesUpToByTrialDivision(N), N);

    [Benchmark]
    public int SieveOfEratosthenes() => NumPrimeArrangements(CountPrimesUpToBySieve(N), N);

    private static int NumPrimeArrangements(int primeCount, int n)
    {
        var compositeCount = n - primeCount;
        return (int)(Factorial(primeCount) * Factorial(compositeCount) % Modulo);
    }

    private static long Factorial(int n)
    {
        var result = 1L;
        for (var i = 2; i <= n; i++)
        {
            result = result * i % Modulo;
        }

        return result;
    }

    private static int CountPrimesUpToByTrialDivision(int n)
    {
        var count = 0;

        for (var candidate = 2; candidate <= n; candidate++)
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

    private static int CountPrimesUpToBySieve(int n)
    {
        if (n < 2)
        {
            return 0;
        }

        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i <= n; i++)
        {
            isComposite.Add(false);
        }

        for (var i = 2; i * i <= n; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= n; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        var count = 0;
        for (var i = 2; i <= n; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
