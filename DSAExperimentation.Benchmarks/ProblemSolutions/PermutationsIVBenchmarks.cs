using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PermutationsIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationsIVSolution's, the same methods
// PermutationsIVTests proves correct. TargetRank is fixed well inside the valid
// range for every measured PermutationLength so both strategies run the full
// unranking walk rather than an early empty-result return.
[MemoryDiagnoser]
public class PermutationsIVBenchmarks
{
    private const long TargetRank = 1_000_000_000_000L;

    [Params(20, 100)]
    public int PermutationLength { get; set; }

    [Benchmark(Baseline = true)]
    public int[] BigIntegerRank() =>
        PermutationsIVSolution.KthPermutationByBigIntegerRank(PermutationLength, TargetRank);

    [Benchmark]
    public int[] FenwickOrderStatistics() =>
        PermutationsIVSolution.KthPermutationByFenwickOrderStatistics(PermutationLength, TargetRank);
}
