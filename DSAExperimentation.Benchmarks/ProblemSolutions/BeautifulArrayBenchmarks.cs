using DSAExperimentation.LeetCode.BeautifulArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BeautifulArraySolution's, the same methods
// BeautifulArraySolutionTests proves correct. The input is a single length, so there is
// nothing to hoist into [GlobalSetup].
//
// Sizes are per arm. The baseline is an exponential backtracking search, so it stops at
// 8; the O(n log n) divide and conquer runs on to LC 932's own bound of 1,000, and the
// two are compared at the sizes both run.
public class BeautifulArrayBenchmarks
{
    public static IEnumerable<int> BaselineSizes => [6, 8];

    public static IEnumerable<int> DivideAndConquerSizes => [.. BaselineSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int[] PrunedBacktrackingSearch(int length) =>
        BeautifulArraySolution.ConstructByPrunedBacktracking(length);

    [Benchmark]
    [ArgumentsSource(nameof(DivideAndConquerSizes))]
    public int[] MemoizedDivideAndConquer(int length) =>
        BeautifulArraySolution.ConstructByMemoizedDivideAndConquer(length);
}
