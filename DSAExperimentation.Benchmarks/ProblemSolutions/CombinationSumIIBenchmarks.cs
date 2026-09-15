using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CombinationSumII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIISolution's, now returning the actual
// combinations the test proves correct instead of merely counting them.
[MemoryDiagnoser]
public class CombinationSumIIBenchmarks
{
    private const int ExampleTarget = 8;
    private static readonly int[] ExampleCandidates = [10, 1, 2, 7, 6, 1, 5];

    [Benchmark(Baseline = true)]
    public List<List<int>> SortAndBacktrackSpecialized() =>
        CombinationSumIISolution.FindCombinationsBySpecializedRecursion(ExampleCandidates, ExampleTarget);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        CombinationSumIISolution.FindCombinationsByBacktracking(ExampleCandidates, ExampleTarget);
}
