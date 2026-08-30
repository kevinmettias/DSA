using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Guess Number Higher or Lower II (LC 375): plain un-memoized minimax recursion over
// (low, high) bounds - exponential, since the same (low, high) sub-range recurs across
// many different choices of guess outside it - vs. this repo's own
// Memoizer<TState,TResult> caching that exact pair (BurstBalloonsBenchmarks' shape).
// N is kept modest specifically because the un-memoized baseline's blowup is real, the
// same reasoning BurstBalloonsBenchmarks/FibonacciBenchmarks already document.
[MemoryDiagnoser]
public class GuessNumberHigherOrLowerIIBenchmarks
{
    [Params(10, 14)]
    public int N;

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => WorstCaseCost(1, N);

    private int WorstCaseCost(int low, int high)
    {
        if (low >= high)
        {
            return 0;
        }

        var best = int.MaxValue;
        for (var guess = low; guess <= high; guess++)
        {
            var worstHalf = Math.Max(WorstCaseCost(low, guess - 1), WorstCaseCost(guess + 1, high));
            best = Math.Min(best, guess + worstHalf);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Low, int High), int>((1, N), WorstCaseCostMemoized);

    private static int WorstCaseCostMemoized((int Low, int High) range, Func<(int Low, int High), int> costFor)
    {
        var (low, high) = range;
        if (low >= high)
        {
            return 0;
        }

        var best = int.MaxValue;
        for (var guess = low; guess <= high; guess++)
        {
            var worstHalf = Math.Max(costFor((low, guess - 1)), costFor((guess + 1, high)));
            best = Math.Min(best, guess + worstHalf);
        }

        return best;
    }
}
