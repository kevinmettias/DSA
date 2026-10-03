using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KokoEatingBananas;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KokoEatingBananasSolution's, the same methods
// KokoEatingBananasSolutionTests proves correct - a hand-rolled lo/hi bisection against
// BinarySearch.LowerBound over an on-demand feasibility sequence. Both binary-search
// the same monotone predicate in O(piles.Length * log(max(piles))), so what is
// measured is the cost of routing it through the reusable
// IRandomAccessSequence<bool> abstraction. The piles are generated once in
// [GlobalSetup].
public class KokoEatingBananasBenchmarks
{
    private const int RandomSeed = 875; // LC problem number
    private const int MaxPileSizeExclusive = 1_000;
    private const int HoursPerBanana = 5;

    private int[] _piles = [];

    private int _hourBudget;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = SeededDraws.Values(Length, 1, MaxPileSizeExclusive, random);
        _hourBudget = Length * HoursPerBanana;
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        KokoEatingBananasSolution.MinEatingSpeedByManualBisection(_piles, _hourBudget);

    [Benchmark]
    public int SequenceLowerBound() =>
        KokoEatingBananasSolution.MinEatingSpeedBySequenceLowerBound(_piles, _hourBudget);
}
