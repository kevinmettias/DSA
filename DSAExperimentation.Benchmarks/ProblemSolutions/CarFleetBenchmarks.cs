using BenchmarkDotNet.Attributes;
using RepoTimeStack = DSAExperimentation.DataStructures.Stack.Stack<double>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Car Fleet (LC 853): both variants start from the same cars-sorted-by-position
// arrival-time array (the O(n log n) sort itself is input setup, not what's
// being compared). RecomputeMaxEachCar re-derives the running "slowest fleet
// ahead" value from scratch for every car via a nested scan, O(n^2).
// MonotonicStackSweep instead composes this repo's own Stack<double>
// (DailyTemperaturesBenchmarks precedent), maintaining that same running
// maximum incrementally as the stack's own top - O(n), one push per car, no
// pop ever needed since arrival times only get compared against the single
// slowest fleet ahead of the current car.
[MemoryDiagnoser]
public class CarFleetBenchmarks
{
    private const int Target = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private double[] _arrivalTimesByPositionDescending = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(853);
        var positions = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, Target))
            .Distinct()
            .OrderByDescending(p => p)
            .ToArray();

        _arrivalTimesByPositionDescending = positions
            .Select(p => (double)(Target - p) / random.Next(1, 100))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecomputeMaxEachCar()
    {
        var times = _arrivalTimesByPositionDescending;
        var fleets = 0;

        for (var i = 0; i < times.Length; i++)
        {
            var slowestAhead = double.NegativeInfinity;

            for (var j = 0; j < i; j++)
            {
                if (times[j] > slowestAhead)
                {
                    slowestAhead = times[j];
                }
            }

            if (times[i] > slowestAhead)
            {
                fleets++;
            }
        }

        return fleets;
    }

    [Benchmark]
    public int MonotonicStackSweep()
    {
        var times = _arrivalTimesByPositionDescending;
        var fleetArrivalTimes = new RepoTimeStack();

        foreach (var time in times)
        {
            if (!fleetArrivalTimes.TryPeek(out var slowestAhead) || time > slowestAhead)
            {
                fleetArrivalTimes.Push(time);
            }
        }

        return fleetArrivalTimes.Count;
    }
}
