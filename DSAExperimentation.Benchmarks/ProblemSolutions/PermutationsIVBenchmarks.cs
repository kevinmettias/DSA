using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PermutationsIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationsIVSolution's, the same methods
// PermutationsIVTests proves correct. K is fixed well inside the valid range for
// every measured N so both strategies run the full unranking walk rather than an
// early empty-result return.
[MemoryDiagnoser]
public class PermutationsIVBenchmarks
{
    private const long K = 1_000_000_000_000L;

    [Params(20, 100)]
    public int N;

    [Benchmark(Baseline = true)]
    public int[] BigIntegerRank() => PermutationsIVSolution.KthPermutationByBigIntegerRank(N, K);

    [Benchmark]
    public int[] FenwickOrderStatistics() => PermutationsIVSolution.KthPermutationByFenwickOrderStatistics(N, K);
}
