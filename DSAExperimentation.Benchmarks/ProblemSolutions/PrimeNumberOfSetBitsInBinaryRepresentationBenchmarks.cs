using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrimeNumberOfSetBitsInBinaryRepresentation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrimeNumberOfSetBitsInBinaryRepresentationSolution's,
// the same methods PrimeNumberOfSetBitsInBinaryRepresentationTests proves correct.
// right <= 10^6 bounds every popcount to at most 20, so the precomputed-set
// strategy's lookup table never grows past 8 entries regardless of range width.
[MemoryDiagnoser]
public class PrimeNumberOfSetBitsInBinaryRepresentationBenchmarks
{
    [Params(1_000, 100_000)]
    public int RangeWidth;

    private int _left;
    private int _right;

    [GlobalSetup]
    public void Setup()
    {
        _left = 1;
        _right = _left + RangeWidth - 1;
    }

    [Benchmark(Baseline = true)]
    public int TrialDivisionPerValue() =>
        PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByTrialDivision(_left, _right);

    [Benchmark]
    public int PrecomputedSetLookup() =>
        PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByPrecomputedSet(_left, _right);
}
