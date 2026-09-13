using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumOneBitOperationsToMakeIntegersZero;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumOneBitOperationsToMakeIntegersZeroSolution's,
// the same methods MinimumOneBitOperationsToMakeIntegersZeroTests proves correct -
// breadth-first search over the implicit Gray-code path graph against the O(log n)
// inverse-Gray-code closed form. The search does work proportional to the answer
// itself (within a small constant factor of n); the closed form only ever touches
// n's own ~log2(n) bits. The input is the single integer LeetCode hands in, so
// there is nothing for a [GlobalSetup] to prepare.
[MemoryDiagnoser]
public class MinimumOneBitOperationsToMakeIntegersZeroBenchmarks
{
    [Params(2_000, 50_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BreadthFirstSearch() =>
        MinimumOneBitOperationsToMakeIntegersZeroSolution.MinimumOneBitOperationsByBreadthFirstSearch(N);

    [Benchmark]
    public int InverseGrayCodeFormula() =>
        MinimumOneBitOperationsToMakeIntegersZeroSolution.MinimumOneBitOperationsByInverseGrayCode(N);
}
