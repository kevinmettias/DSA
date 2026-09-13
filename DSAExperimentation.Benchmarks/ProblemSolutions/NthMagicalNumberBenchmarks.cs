using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NthMagicalNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NthMagicalNumberSolution's, the same methods
// NthMagicalNumberTests proves correct. a = 6 and b = 10 share a factor, so the
// inclusion-exclusion term does real work instead of collapsing to zero, and
// counting candidates one at a time (O(answer)) is measured against
// BinarySearch.LowerBound over the monotone "count(x) >= n" sequence
// (O(log(answer))).
[MemoryDiagnoser]
public class NthMagicalNumberBenchmarks
{
    private const int A = 6;
    private const int B = 10;

    [Params(2_000, 50_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BruteForceCount() => NthMagicalNumberSolution.NthMagicalNumberByCountScan(N, A, B);

    [Benchmark]
    public int BinarySearchOnCount() =>
        NthMagicalNumberSolution.NthMagicalNumberByBinarySearch(N, A, B);
}
