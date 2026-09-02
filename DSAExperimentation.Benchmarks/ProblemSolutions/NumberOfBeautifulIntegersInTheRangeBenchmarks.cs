using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfBeautifulIntegersInTheRange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfBeautifulIntegersInTheRangeSolution's, the
// same methods NumberOfBeautifulIntegersInTheRangeTests proves correct
// (TwoSumBenchmarks precedent). CountByDigitDpMemo's cost depends only on High's
// own digit count (~constant here, both Params share the same digit length), while
// CountByBruteForce walks every integer in [Low, High] - RangeSize is what
// actually drives the gap wider as it grows.
[MemoryDiagnoser]
public class NumberOfBeautifulIntegersInTheRangeBenchmarks
{
    private const int Low = 1;
    private const int K = 7;

    [Params(100_000, 1_000_000)]
    public int RangeSize;

    private int _high;

    [GlobalSetup]
    public void Setup() => _high = Low + RangeSize;

    [Benchmark(Baseline = true)]
    public long BruteForce() => NumberOfBeautifulIntegersInTheRangeSolution.CountByBruteForce(Low, _high, K);

    [Benchmark]
    public long DigitDpMemo() => NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo(Low, _high, K);
}
