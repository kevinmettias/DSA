using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.CarFleet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CarFleetSolution's, the same methods CarFleetTests
// proves correct. [GlobalSetup] builds the cars-sorted-by-position arrival-time list
// once - the O(n log n) sort is input setup, not what the two strategies differ in -
// and hands it to each strategy's prepared-input overload (ARCHITECTURE.md §17.4), so
// only the running-maximum work is measured.
[MemoryDiagnoser]
public class CarFleetBenchmarks
{
    private const int Target = 1_000_000;

    // LeetCode problem number for Car Fleet.
    private const int RandomSeed = 853;

    private const int MaxRandomSpeedDivisorExclusive = 100;

    private DynamicArray<double> _arrivalTimesByPositionDescending = new();

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var positions = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, Target))
            .Distinct()
            .ToArray();

        var speeds = positions.Select(_ => random.Next(1, MaxRandomSpeedDivisorExclusive)).ToArray();

        _arrivalTimesByPositionDescending =
            CarFleetSolution.ArrivalTimesByPositionDescending(Target, positions, speeds);
    }

    [Benchmark(Baseline = true)]
    public int RecomputeMaxEachCar()
        => CarFleetSolution.CountFleetsByRecomputeMaxEachCar(_arrivalTimesByPositionDescending);

    [Benchmark]
    public int MonotonicStackSweep()
        => CarFleetSolution.CountFleetsByMonotonicStackSweep(_arrivalTimesByPositionDescending);
}
