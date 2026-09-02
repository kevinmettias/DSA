using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoRangeFenwickTree = DSAExperimentation.DataStructures.RangeFenwickTree.RangeFenwickTree<long, DSAExperimentation.DataStructures.RangeFenwickTree.ScaledSumOperation<long>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximize the Minimum Powered City (LC 2528): a descending linear scan over
// every candidate target vs. this repo's own BinarySearch.LowerBound over an
// on-demand IRandomAccessSequence<bool> feasibility sequence
// (MaximizeTheMinimumPoweredCityTests precedent, same "maximize the answer"
// shape MaximumNumberOfTasksYouCanAssignBenchmarks/KokoEatingBananasBenchmarks
// already run) - both call the identical RangeFenwickTree-backed feasibility
// check, just O(upperBound) times for the linear scan vs. O(log upperBound)
// times for the binary search. Station values and k are kept modest so
// upperBound (= sum(stations) + k) stays in the low thousands - large enough
// to separate the two strategies, small enough that the linear scan's full
// descent still finishes quickly.
[MemoryDiagnoser]
public class MaximizeTheMinimumPoweredCityBenchmarks
{
    private const int RandomSeed = 2528; // LC problem number
    private const int MaxStationValueExclusive = 20;
    private const int Range = 1;
    private const int ExtraStations = 5;

    [Params(50, 300)]
    public int Length;

    private int[] _stations = null!;
    private long _upperBound;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stations = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxStationValueExclusive)).ToArray();
        _upperBound = _stations.Sum(s => (long)s) + ExtraStations;
    }

    [Benchmark(Baseline = true)]
    public long LinearScan()
    {
        for (var target = _upperBound; target >= 0; target--)
        {
            if (Feasible(_stations, Range, ExtraStations, target))
            {
                return target;
            }
        }

        return 0;
    }

    [Benchmark]
    public long SequenceLowerBound()
    {
        var sequence = new InfeasibleSequence(_stations, Range, ExtraStations, _upperBound);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    private static bool Feasible(int[] stations, int r, long k, long target)
    {
        var n = stations.Length;
        var tree = new RepoRangeFenwickTree(n);

        for (var i = 0; i < n; i++)
        {
            tree.RangeAdd(Math.Max(0, i - r), Math.Min(n - 1, i + r), stations[i]);
        }

        var remaining = k;

        for (var i = 0; i < n; i++)
        {
            var current = tree.Query(i, i);
            if (current >= target)
            {
                continue;
            }

            var need = target - current;
            if (need > remaining)
            {
                return false;
            }

            remaining -= need;
            var pos = Math.Min(n - 1, i + r);
            tree.RangeAdd(Math.Max(0, pos - r), Math.Min(n - 1, pos + r), need);
        }

        return true;
    }

    private readonly struct InfeasibleSequence(int[] stations, int r, long k, long upperBound)
        : IRandomAccessSequence<bool>
    {
        public int Length => (int)upperBound + 1;

        public bool Get(int index) => !Feasible(stations, r, k, index);
    }
}
