using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Fibonacci Number (LC 509): the naive O(2^n) double-recursion vs. this repo's
// Memoizer-backed O(n) top-down DP - the same ClimbingStairsTests precedent, wired
// up as a head-to-head benchmark here instead of a coverage test.
[MemoryDiagnoser]
public class FibonacciNumberBenchmarks
{
    // Offset back to the second predecessor in the Fibonacci recurrence fib(n-1) + fib(n-2).
    private const int SecondPredecessorOffset = 2;

    [Params(20, 30)]
    public int N;

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => Fib(N);

    private static int Fib(int n) => n <= 1 ? n : Fib(n - 1) + Fib(n - SecondPredecessorOffset);

    [Benchmark]
    public int MemoizedTopDown()
        => Memoizer.Memoize<int, int>(
            N,
            (value, fib) => value <= 1 ? value : fib(value - 1) + fib(value - SecondPredecessorOffset));
}
