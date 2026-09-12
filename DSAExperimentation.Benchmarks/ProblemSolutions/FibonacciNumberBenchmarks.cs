using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FibonacciNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FibonacciNumberSolution's, the same methods
// FibonacciNumberTests proves correct - the naive O(2^n) double-recursion baseline
// vs. this repo's Memoizer-backed O(n) top-down DP.
[MemoryDiagnoser]
public class FibonacciNumberBenchmarks
{
    [Params(20, 30)]
    public int N;

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => FibonacciNumberSolution.FibByNaiveRecursion(N);

    [Benchmark]
    public int MemoizedTopDown() => FibonacciNumberSolution.FibByMemoizedTopDown(N);
}
