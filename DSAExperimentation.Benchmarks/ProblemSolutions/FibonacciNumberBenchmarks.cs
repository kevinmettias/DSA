using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FibonacciNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is FibonacciNumberSolution's, the same methods
// FibonacciNumberTests proves correct - the naive O(2^n) double-recursion baseline
// vs. this repo's Memoizer-backed O(n) top-down DP vs. the O(1)-space rolling pair.
// SequenceIndex is kept modest (<=30) because the baseline's blowup is real, not
// because the other two arms need it. The same recurrence is Climbing Stairs'
// (LC 70), which ClimbingStairsBenchmarks measures on its own terms.
[MemoryDiagnoser]
public class FibonacciNumberBenchmarks
{
    [Params(20, 30)]
    public int SequenceIndex { get; set; }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => FibonacciNumberSolution.FibByNaiveRecursion(SequenceIndex);

    [Benchmark]
    public int MemoizedTopDown() => FibonacciNumberSolution.FibByMemoizedTopDown(SequenceIndex);

    [Benchmark]
    public int IterativeRollingPair() => FibonacciNumberSolution.FibByIterativeRollingPair(SequenceIndex);
}
