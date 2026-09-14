using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimizeTheMaximumOfTwoArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeTheMaximumOfTwoArraysSolution's, the same
// methods MinimizeTheMaximumOfTwoArraysTests proves correct - a hand-rolled lo/hi
// bisection against BinarySearch.LowerBound over an on-demand feasibility sequence.
// Both binary-search the same monotone predicate over the same range, so what is
// measured is the cost of routing it through the reusable IRandomAccessSequence
// abstraction. Divisors are fixed and coprime (2, 3) so every UniqueCountScale
// forces a real lcm=6 inclusion-exclusion check instead of degenerating to a
// single-divisor case.
[MemoryDiagnoser]
public class MinimizeTheMaximumOfTwoArraysBenchmarks
{
    private const int Divisor1 = 2;
    private const int Divisor2 = 3;

    [Params(1_000, 1_000_000)]
    public int UniqueCountScale;

    private int _uniqueCnt1;
    private int _uniqueCnt2;

    [GlobalSetup]
    public void Setup()
    {
        _uniqueCnt1 = UniqueCountScale;
        _uniqueCnt2 = UniqueCountScale;
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetByManualBisection(
            Divisor1, Divisor2, _uniqueCnt1, _uniqueCnt2);

    [Benchmark]
    public int SequenceLowerBound() =>
        MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetBySequenceLowerBound(
            Divisor1, Divisor2, _uniqueCnt1, _uniqueCnt2);
}
