using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumOperationsToConvertAllElementsToZero;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumOperationsToConvertAllElementsToZeroSolution's,
// the same methods MinimumOperationsToConvertAllElementsToZeroTests proves correct.
//
// The workload is strictly increasing (1, 2, ..., Length), the divide-and-conquer
// strategy's worst case: the range minimum sits at the very start of every
// remaining segment, so each level of recursion only ever peels off one element
// after an O(remaining) scan, making the whole walk O(n^2). The monotonic-stack
// strategy processes the same input in one O(n) pass.
[MemoryDiagnoser]
public class MinimumOperationsToConvertAllElementsToZeroBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int DivideAndConquer() =>
        MinimumOperationsToConvertAllElementsToZeroSolution.MinOperationsByDivideAndConquer(_values);

    [Benchmark]
    public int MonotonicStack() =>
        MinimumOperationsToConvertAllElementsToZeroSolution.MinOperationsByMonotonicStack(_values);
}
