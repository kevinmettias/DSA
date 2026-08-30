using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Non-overlapping Intervals (LC 435): the O(n^2) brute force (repeatedly rescan
// every still-active interval for the smallest end, keep it, then eliminate
// every remaining interval it strictly overlaps) vs. this repo's own O(n log n)
// MergeSort over ArrayIndexedSequence to sort once by end, followed by a single
// O(n) greedy pass (NonOverlappingIntervalsTests' algorithm) - the same
// sort-then-greedy composition MinimumNumberOfArrowsToBurstBalloonsBenchmarks
// already proves, with the overlap comparison flipped ('>=' keeps touching
// intervals, since LC 435 - unlike LC 452 - does not treat a shared endpoint as
// overlapping). _intervals is generated as mostly non-overlapping, shuffled
// intervals (few removals needed), which hits brute force's true worst case -
// its outer "find the next kept interval" loop runs close to n times, each
// paying a full O(n) rescan - instead of the heavy-overlap case where most
// intervals get eliminated in the first few rounds.
[MemoryDiagnoser]
public class NonOverlappingIntervalsBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private (int Start, int End)[] _intervals = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(435);
        _intervals = Enumerable.Range(0, Length)
            .Select(i => (Start: i * 3, End: i * 3 + random.Next(0, 2)))
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedMinEndScan()
    {
        var eliminated = new bool[_intervals.Length];
        var remaining = _intervals.Length;
        var removedCount = 0;

        while (remaining > 0)
        {
            var minEnd = int.MaxValue;
            var minIndex = -1;

            for (var i = 0; i < _intervals.Length; i++)
            {
                if (!eliminated[i] && _intervals[i].End < minEnd)
                {
                    minEnd = _intervals[i].End;
                    minIndex = i;
                }
            }

            eliminated[minIndex] = true;
            remaining--;

            for (var i = 0; i < _intervals.Length; i++)
            {
                if (!eliminated[i] && _intervals[i].Start < minEnd)
                {
                    eliminated[i] = true;
                    remaining--;
                    removedCount++;
                }
            }
        }

        return removedCount;
    }

    [Benchmark]
    public int SortByEndThenGreedyScan()
    {
        var items = ((int Start, int End)[])_intervals.Clone();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(items),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var kept = 1;
        var lastEnd = items[0].End;

        for (var i = 1; i < items.Length; i++)
        {
            if (items[i].Start >= lastEnd)
            {
                kept++;
                lastEnd = items[i].End;
            }
        }

        return items.Length - kept;
    }
}
