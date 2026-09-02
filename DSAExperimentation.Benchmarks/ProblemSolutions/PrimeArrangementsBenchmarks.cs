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

    // 2 is the smallest prime; every prime-related loop or threshold starts here.
    private const int SmallestPrime = 2;

    // Factorial multiplication only needs to start at 2 - 0! and 1! are both 1.
    private const int FactorialLoopStart = 2;

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
        for (var i = FactorialLoopStart; i <= n; i++)
        {
            result = result * i % Modulo;
        }

        return result;
    }

    private static int CountPrimesUpToByTrialDivision(int n)
    {
        var count = 0;

        for (var candidate = SmallestPrime; candidate <= n; candidate++)
        {
            var isPrime = true;

            for (var divisor = SmallestPrime; divisor * divisor <= candidate; divisor++)
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
        if (n < SmallestPrime)
        {
            return 0;
        }

        var isComposite = BuildSieve(n);
        MarkComposites(isComposite, n);
        return CountUnmarked(isComposite, n);
    }

    private static DynamicArray<bool> BuildSieve(int n)
    {
        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i <= n; i++)
        {
            isComposite.Add(false);
        }

        return isComposite;
    }

    private static void MarkComposites(DynamicArray<bool> isComposite, int n)
    {
        for (var i = SmallestPrime; i * i <= n; i++)
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
    }

    private static int CountUnmarked(DynamicArray<bool> isComposite, int n)
    {
        var count = 0;
        for (var i = SmallestPrime; i <= n; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
