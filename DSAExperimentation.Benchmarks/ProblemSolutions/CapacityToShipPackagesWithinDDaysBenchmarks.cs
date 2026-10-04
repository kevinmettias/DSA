using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CapacityToShipPackagesWithinDDays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CapacityToShipPackagesWithinDDaysSolution's, the same
// methods CapacityToShipPackagesWithinDDaysSolutionTests proves correct - a hand-rolled lo/hi
// bisection against MonotonePredicateSearch.FirstTrue over the capacity range.
// Both binary-search the same monotone feasibility
// predicate in O(weights.Length * log(sum - max)), so what is measured is the cost of
// routing it through the reusable abstraction. The weights are generated once in
// [GlobalSetup].
public class CapacityToShipPackagesWithinDDaysBenchmarks
{
    private const int RandomSeed = 1011; // LC problem number
    // LC 1011's weights are at most 500.
    private const int MaxWeightExclusive = 501;
    private const int DaysDivisor = 20;

    private int[] _weights = [];

    private int _days;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _weights = SeededDraws.Values(Length, 1, MaxWeightExclusive, random);
        _days = Math.Max(1, Length / DaysDivisor);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysByManualBisection(_weights, _days);

    [Benchmark]
    public int PredicateSearch() =>
        CapacityToShipPackagesWithinDDaysSolution.ShipWithinDaysByPredicateSearch(_weights, _days);
}
