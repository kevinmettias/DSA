using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Capacity To Ship Packages Within D Days (LC 1011): a hand-rolled int lo/hi
// bisection loop vs. this repo's own BinarySearch.LowerBound over an on-demand
// FeasibleCapacitySequence (CapacityToShipPackagesWithinDDaysTests precedent,
// itself the same "binary search on the answer" shape SplitArrayLargestSumBenchmarks
// already uses) - both binary-search the same monotone feasibility predicate in
// O(weights.Length * log(sum - max)), just one through a bespoke loop and the other
// through the reusable IRandomAccessSequence<bool> abstraction.
[MemoryDiagnoser]
public class CapacityToShipPackagesWithinDDaysBenchmarks
{
    private const int RandomSeed = 1011; // LC problem number
    private const int MaxWeightExclusive = 1_000;
    private const int DaysDivisor = 20;
    private const int MidpointDivisor = 2;

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
    public int ManualBinarySearch()
    {
        var low = _weights.Max();
        var high = _weights.Sum();

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);
            if (CanShipWithinDays(_weights, _days, mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var floor = _weights.Max();
        var ceiling = _weights.Sum();
        var sequence = new FeasibleCapacitySequence(_weights, _days, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

    private static bool CanShipWithinDays(int[] weights, int days, int capacity)
    {
        var daysNeeded = 1;
        var currentLoad = 0;

        foreach (var weight in weights)
        {
            if (currentLoad + weight > capacity)
            {
                daysNeeded++;
                currentLoad = 0;
            }

            currentLoad += weight;
        }

        return daysNeeded <= days;
    }

    private readonly struct FeasibleCapacitySequence(int[] weights, int days, int floor, int ceiling)
        : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanShipWithinDays(weights, days, floor + index);
    }
}
