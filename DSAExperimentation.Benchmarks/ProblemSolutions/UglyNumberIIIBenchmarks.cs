using DSAExperimentation.LeetCode.UglyNumberIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UglyNumberIIISolution's, the same methods
// UglyNumberIIISolutionTests proves correct. Counting candidates one at a time
// (O(answer)) is measured against MonotonePredicateSearch.FirstTrue over the
// monotone "count(x) >= rank" rule (O(log(answer))) - the same shape
// NthMagicalNumberBenchmarks already exercises, extended from a two-term to a
// three-term inclusion-exclusion predicate per candidate.
public class UglyNumberIIIBenchmarks
{
    private const int FirstFactor = 2;
    private const int SecondFactor = 3;
    private const int ThirdFactor = 5;

    [Params(2_000, 50_000)]
    public int Rank { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteForceCount() =>
        UglyNumberIIISolution.NthUglyNumberByCountScan(Rank, FirstFactor, SecondFactor, ThirdFactor);

    [Benchmark]
    public int BinarySearchOnCount() =>
        UglyNumberIIISolution.NthUglyNumberByBinarySearch(Rank, FirstFactor, SecondFactor, ThirdFactor);
}
