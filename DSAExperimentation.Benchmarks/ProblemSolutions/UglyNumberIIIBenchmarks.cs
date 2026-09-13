using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UglyNumberIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UglyNumberIIISolution's, the same methods
// UglyNumberIIITests proves correct. Counting candidates one at a time
// (O(answer)) is measured against BinarySearch.LowerBound over the monotone
// "count(x) >= n" virtual sequence (O(log(answer))) - the same shape
// NthMagicalNumberBenchmarks already exercises, extended from a two-term to a
// three-term inclusion-exclusion predicate per index.
[MemoryDiagnoser]
public class UglyNumberIIIBenchmarks
{
    private const int A = 2;
    private const int B = 3;
    private const int C = 5;

    [Params(2_000, 50_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BruteForceCount() => UglyNumberIIISolution.NthUglyNumberByCountScan(N, A, B, C);

    [Benchmark]
    public int BinarySearchOnCount() =>
        UglyNumberIIISolution.NthUglyNumberByBinarySearch(N, A, B, C);
}
