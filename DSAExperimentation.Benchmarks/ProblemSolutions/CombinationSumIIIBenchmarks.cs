using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CombinationSumIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIIISolution's, now returning the actual
// combinations the test proves correct instead of the compile-smoke placeholder
// this class previously was. K is fixed at 5 (the richest branching factor over the
// 1..9 domain); N is varied around the midpoint of that width's achievable sums so
// neither Params value trivially empties the result.
[MemoryDiagnoser]
public class CombinationSumIIIBenchmarks
{
    private const int K = 5;

    [Params(20, 25)]
    public int N;

    [Benchmark(Baseline = true)]
    public List<List<int>> BruteForce() =>
        CombinationSumIIISolution.CombinationsByBruteForce(K, N);

    [Benchmark]
    public List<List<int>> BacktrackEngine() =>
        CombinationSumIIISolution.CombinationsByBacktrackEngine(K, N);
}
