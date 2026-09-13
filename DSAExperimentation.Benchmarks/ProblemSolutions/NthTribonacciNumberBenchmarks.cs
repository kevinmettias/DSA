using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NthTribonacciNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NthTribonacciNumberSolution's, the same methods
// NthTribonacciNumberTests proves correct - the naive triple recursion vs. this
// repo's Memoizer-backed O(n) top-down DP. The same shape as
// FibonacciNumberBenchmarks, with three prior terms summed instead of two.
[MemoryDiagnoser]
public class NthTribonacciNumberBenchmarks
{
    [Params(20, 30)]
    public int N;

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => NthTribonacciNumberSolution.TribonacciByNaiveRecursion(N);

    [Benchmark]
    public int MemoizedTopDown() => NthTribonacciNumberSolution.TribonacciByMemoizedTopDown(N);
}
