using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// N-th Tribonacci Number (LC 1137): the same naive-recursion-vs-Memoizer
// shape as FibonacciNumberBenchmarks, just with three prior terms summed
// instead of two.
[MemoryDiagnoser]
public class NthTribonacciNumberBenchmarks
{
    [Params(20, 30)]
    public int N;

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => Tribonacci(N);

    private static int Tribonacci(int n)
        => n switch
        {
            0 => 0,
            1 or 2 => 1,
            _ => Tribonacci(n - 1) + Tribonacci(n - 2) + Tribonacci(n - 3)
        };

    [Benchmark]
    public int MemoizedTopDown()
        => Memoizer.Memoize<int, int>(
            N,
            (value, trib) => value switch
            {
                0 => 0,
                1 or 2 => 1,
                _ => trib(value - 1) + trib(value - 2) + trib(value - 3)
            });
}
