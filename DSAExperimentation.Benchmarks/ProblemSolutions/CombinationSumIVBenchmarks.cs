using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CombinationSumIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIVSolution's, the same methods
// CombinationSumIVTests proves correct. The running counts overflow a 32-bit int well
// before Target's upper [Params] bound (the real LeetCode judge only guarantees an
// int-sized answer for its own, much smaller constraints) - harmless here since both
// benchmarked methods overflow identically and this class measures wall-clock time,
// not the returned value.
[MemoryDiagnoser]
public class CombinationSumIVBenchmarks
{
    private static readonly int[] Nums = [1, 2, 3, 5, 10];

    [Params(200, 2_000)]
    public int Target { get; set; }

    [Benchmark(Baseline = true)]
    public int Tabulation() => CombinationSumIVSolution.CountCombinationsByTabulation(Nums, Target);

    [Benchmark]
    public int Memoized() => CombinationSumIVSolution.CountCombinationsByMemoizedRecursion(Nums, Target);
}
