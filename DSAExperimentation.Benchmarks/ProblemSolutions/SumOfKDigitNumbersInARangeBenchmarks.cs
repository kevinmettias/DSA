using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfKDigitNumbersInARange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfKDigitNumbersInARangeSolution's, the same
// methods SumOfKDigitNumbersInARangeTests proves correct. l/r are fixed at the
// widest possible digit range (0-9, so c = 10) to give the brute-force arm's
// c^K enumeration its worst branching factor; K is the only [Params] axis,
// kept small enough for that enumeration to finish - the modular-repunit arm
// would stay just as fast at LeetCode's real K <= 1e9.
[MemoryDiagnoser]
public class SumOfKDigitNumbersInARangeBenchmarks
{
    private const int Low = 0;
    private const int High = 9;

    [Params(4, 6)]
    public int K;

    [Benchmark(Baseline = true)]
    public long BruteForceEnumeration() =>
        SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByBruteForceEnumeration(Low, High, K);

    [Benchmark]
    public long ModularRepunit() =>
        SumOfKDigitNumbersInARangeSolution.SumOfKDigitNumbersByModularRepunit(Low, High, K);
}
