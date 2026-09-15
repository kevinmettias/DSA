using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheCountOfGoodIntegersSolution's, the same methods
// FindTheCountOfGoodIntegersTests proves correct.
[MemoryDiagnoser]
public class FindTheCountOfGoodIntegersBenchmarks
{
    private const int K = 6;

    [Params(6, 10)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public long PalindromeEnumeration() => FindTheCountOfGoodIntegersSolution.CountByPalindromeEnumeration(N, K);

    [Benchmark]
    public long BacktrackEnumeration() => FindTheCountOfGoodIntegersSolution.CountByBacktrackEnumeration(N, K);
}
