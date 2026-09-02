using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Final Prices With a Special Discount in a Shop (LC 1475): the O(n^2) brute-force
// nested-loop search for the next not-greater price vs. the O(n) monotonic-stack
// approach using this repo's own Stack<int> (LIFO over DynamicArray<int>,
// ARCHITECTURE.md §4.1) - the same brute-force-vs-primitive shape as
// TwoSumBenchmarks. Prices are random with no forced worst case, matching the
// distribution LeetCode's own constraints describe.
[MemoryDiagnoser]
public class FinalPricesWithASpecialDiscountInAShopBenchmarks
{
    private const int MaxPrice = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPrice)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var result = (int[])_prices.Clone();

        for (var i = 0; i < _prices.Length; i++)
        {
            for (var j = i + 1; j < _prices.Length; j++)
            {
                if (_prices[j] <= _prices[i])
                {
                    result[i] -= _prices[j];
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] MonotonicStack()
    {
        var result = (int[])_prices.Clone();
        var pendingIndices = new RepoStack();

        for (var i = 0; i < _prices.Length; i++)
        {
            while (pendingIndices.TryPeek(out var top) && _prices[i] <= _prices[top])
            {
                pendingIndices.TryPop(out _);
                result[top] -= _prices[i];
            }

            pendingIndices.Push(i);
        }

        return result;
    }
}
