using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CombinationSumIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIIISolution's, now returning the actual
// combinations the test proves correct instead of the compile-smoke placeholder
// this class previously was. CombinationSize is fixed at 5 (the richest branching
// factor over the 1..9 domain); TargetSum is varied around the midpoint of that
// width's achievable sums so neither Params value trivially empties the result.
[MemoryDiagnoser]
public class CombinationSumIIIBenchmarks
{
    private const int CombinationSize = 5;

    [Params(20, 25)]
    public int TargetSum { get; set; }

    [Benchmark(Baseline = true)]
    public List<List<int>> BruteForce() =>
        CombinationSumIIISolution.CombinationsByBruteForce(CombinationSize, TargetSum);

    [Benchmark]
    public List<List<int>> BacktrackEngine() =>
        CombinationSumIIISolution.CombinationsByBacktrackEngine(CombinationSize, TargetSum);
}
