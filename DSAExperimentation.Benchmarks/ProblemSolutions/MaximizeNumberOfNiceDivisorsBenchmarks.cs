using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeNumberOfNiceDivisors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeNumberOfNiceDivisorsSolution's, the same
// methods MaximizeNumberOfNiceDivisorsTests proves correct - the un-memoized
// recursion that re-explores every peel-2/peel-3 order (the same "no cache" shape
// FibonacciNumberBenchmarks' own baseline uses) against this repo's own Memoizer
// keyed on the int `remaining` state. PrimeFactors stays small enough that the
// naive side's ~1.33^n blowup still finishes in reasonable time while remaining
// clearly exponential next to the memoized side's O(n).
[MemoryDiagnoser]
public class MaximizeNumberOfNiceDivisorsBenchmarks
{
    [Params(40, 50)]
    public int PrimeFactors { get; set; }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() =>
        MaximizeNumberOfNiceDivisorsSolution.MaxNiceDivisorsByNaiveRecursion(PrimeFactors);

    [Benchmark]
    public int MemoizedTopDown() =>
        MaximizeNumberOfNiceDivisorsSolution.MaxNiceDivisorsByMemoizedRecurrence(PrimeFactors);
}
