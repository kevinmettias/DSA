using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheNumberOfIdealArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfIdealArraysSolution's, the same methods
// CountTheNumberOfIdealArraysTests proves correct. Trial division re-pays O(sqrt(value))
// for every value from 1 to MaxValue; the smallest-prime-factor sieve pays
// O(MaxValue log log MaxValue) once and amortizes it across all MaxValue factorizations,
// so the sieve build stays inside the measured method - it is the cost being amortized,
// not setup. The array length is fixed so the only thing varying is the factoring work.
[MemoryDiagnoser]
public class CountTheNumberOfIdealArraysBenchmarks
{
    // Fixed array length; only the value range varies across the measured runs.
    private const int N = 4;

    [Params(200, 5_000)]
    public int MaxValue { get; set; }

    [Benchmark(Baseline = true)]
    public int TrialDivisionPerValue() =>
        CountTheNumberOfIdealArraysSolution.IdealArraysByTrialDivision(N, MaxValue);

    [Benchmark]
    public int SmallestPrimeFactorSieve() =>
        CountTheNumberOfIdealArraysSolution.IdealArraysBySmallestPrimeFactorSieve(N, MaxValue);
}
