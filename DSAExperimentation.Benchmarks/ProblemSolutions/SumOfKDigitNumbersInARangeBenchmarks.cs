using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfKDigitNumbersInARange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfKDigitNumbersInARangeSolution's, the same
// methods SumOfKDigitNumbersInARangeTests proves correct. The digit range Low..High
// is fixed at its widest (0-9, ten choices per digit) to give the brute-force arm's
// exponential enumeration its worst branching factor; DigitCount is the only
// [Params] axis, kept small enough for that enumeration to finish - the
// modular-repunit arm would stay just as fast at LeetCode's real K <= 1e9.
[MemoryDiagnoser]
public class SumOfKDigitNumbersInARangeBenchmarks
{
    private const int Low = 0;
    private const int High = 9;

    [Params(4, 6)]
    public int DigitCount { get; set; }

    [Benchmark(Baseline = true)]
    public long BruteForceEnumeration() =>
        SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByBruteForceEnumeration(Low, High, DigitCount);

    [Benchmark]
    public long ModularRepunit() =>
        SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByModularRepunit(Low, High, DigitCount);
}
