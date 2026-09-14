using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.MaximizeTheMinimumPoweredCity.MaximizeTheMinimumPoweredCitySolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeTheMinimumPoweredCitySolution's, the same
// methods MaximizeTheMinimumPoweredCityTests proves correct - a descending linear scan
// over every candidate target against this repo's own BinarySearch.LowerBound over an
// on-demand IRandomAccessSequence<bool> feasibility sequence, the same
// search-on-the-answer shape MaximumNumberOfTasksYouCanAssignBenchmarks and
// KokoEatingBananasBenchmarks already run, so the comparison is O(upperBound)
// feasibility sweeps against O(log upperBound) of them. Station values and the station
// budget are kept modest so upperBound (= sum(stations) + k) stays in the low thousands
// - large enough to separate the two strategies, small enough that the linear scan's
// full descent still finishes quickly. [GlobalSetup] builds the prepared
// PoweredCityPlan its hoisted overload takes, so generating and totalling the stations
// is not charged to either measured arm.
[MemoryDiagnoser]
public class MaximizeTheMinimumPoweredCityBenchmarks
{
    private const int RandomSeed = 2528; // LC problem number
    private const int MaxStationValueExclusive = 20;
    private const int Range = 1;
    private const int ExtraStations = 5;

    [Params(50, 300)]
    public int Length;

    private PoweredCityPlan _plan;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var stations = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, MaxStationValueExclusive))
            .ToArray();

        _plan = PoweredCityPlan.From(stations, Range, ExtraStations);
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => MaxPowerByDescendingLinearScan(_plan);

    [Benchmark]
    public long SequenceLowerBound() => MaxPowerBySequenceLowerBound(_plan);
}
