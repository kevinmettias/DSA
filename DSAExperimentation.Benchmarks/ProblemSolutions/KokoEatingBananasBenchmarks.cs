using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KokoEatingBananas;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KokoEatingBananasSolution's, the same methods
// KokoEatingBananasTests proves correct - a hand-rolled lo/hi bisection against
// BinarySearch.LowerBound over an on-demand feasibility sequence. Both binary-search
// the same monotone predicate in O(piles.Length * log(max(piles))), so what is
// measured is the cost of routing it through the reusable
// IRandomAccessSequence<bool> abstraction. The piles are generated once in
// [GlobalSetup].
[MemoryDiagnoser]
public class KokoEatingBananasBenchmarks
{
    private const int RandomSeed = 875; // LC problem number
    private const int MaxPileSizeExclusive = 1_000;
    private const int HoursPerBanana = 5;

    [Params(200, 5_000)]
    public int Length;

    private int[] _piles = null!;
    private int _h;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPileSizeExclusive)).ToArray();
        _h = Length * HoursPerBanana;
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        KokoEatingBananasSolution.MinEatingSpeedByManualBisection(_piles, _h);

    [Benchmark]
    public int SequenceLowerBound() =>
        KokoEatingBananasSolution.MinEatingSpeedBySequenceLowerBound(_piles, _h);
}
