using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BeautifulArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BeautifulArraySolution's, the same methods
// BeautifulArrayTests proves correct. The input is a single length, so there is
// nothing to hoist into [GlobalSetup] - the sizes stay small because the baseline
// is an exponential backtracking search.
[MemoryDiagnoser]
public class BeautifulArrayBenchmarks
{
    [Params(6, 8)]
    public int Length;

    [Benchmark(Baseline = true)]
    public int[] PrunedBacktrackingSearch() =>
        BeautifulArraySolution.ConstructByPrunedBacktracking(Length);

    [Benchmark]
    public int[] MemoizedDivideAndConquer() =>
        BeautifulArraySolution.ConstructByMemoizedDivideAndConquer(Length);
}
