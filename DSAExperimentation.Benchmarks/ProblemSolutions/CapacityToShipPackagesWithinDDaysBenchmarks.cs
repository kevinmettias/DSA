using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CapacityToShipPackagesWithinDDays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CapacityToShipPackagesWithinDDaysSolution's, the same
// methods CapacityToShipPackagesWithinDDaysTests proves correct - a hand-rolled lo/hi
// bisection against BinarySearch.LowerBound over an on-demand
// IRandomAccessSequence<bool>. Both binary-search the same monotone feasibility
// predicate in O(weights.Length * log(sum - max)), so what is measured is the cost of
// routing it through the reusable abstraction. The weights are generated once in
// [GlobalSetup].
[MemoryDiagnoser]
public class CapacityToShipPackagesWithinDDaysBenchmarks
{
    private const int RandomSeed = 1011; // LC problem number
    private const int MaxWeightExclusive = 1_000;
    private const int DaysDivisor = 20;

    [Params(200, 5_000)]
    public int Length;

    private int[] _weights = null!;
    private int _days;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _weights = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxWeightExclusive)).ToArray();
        _days = Math.Max(1, Length / DaysDivisor);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysByManualBisection(_weights, _days);

    [Benchmark]
    public int SequenceLowerBound() =>
        CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysBySequenceLowerBound(_weights, _days);
}
