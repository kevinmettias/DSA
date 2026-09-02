using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// My Calendar III (LC 732): the textbook brute force (on every booking, rescan
// every start point ever seen against every booking recorded so far to find the
// worst simultaneous overlap - O(n) event points times O(n) bookings times O(n)
// bookings again) vs. this repo's own HashMap<int,int> delta accumulation plus
// MergeSort.Sort over an ArrayIndexedSequence<int> to re-sort the event points
// each call - O(n log n) per booking instead of O(n^2). Bookings never get
// removed, so the running max returned by the final call already equals the
// max over the whole run, letting both strategies return one comparable int.
[MemoryDiagnoser]
public class MyCalendarIIIBenchmarks
{
    // LC 732.
    private const int RandomSeed = 732;
    private const int StartRangeMultiplier = 2;
    private const int MaxBookingDuration = 20;

    [Params(50, 200)]
    public int Length;

    private (int Start, int End)[] _bookings = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _bookings = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var start = random.Next(0, Length * StartRangeMultiplier);
                var end = start + random.Next(1, MaxBookingDuration);
                return (Start: start, End: end);
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceEventRescan()
    {
        var bookings = new List<(int Start, int End)>();
        var maxOverlap = 0;

        foreach (var booking in _bookings)
        {
            bookings.Add(booking);
            maxOverlap = Math.Max(maxOverlap, MaxOverlapAcrossStarts(bookings));
        }

        return maxOverlap;
    }

    private static int MaxOverlapAcrossStarts(List<(int Start, int End)> bookings)
    {
        var maxOverlap = 0;

        foreach (var (candidateStart, _) in bookings)
        {
            var overlapCount = OverlapCountAt(candidateStart, bookings);
            maxOverlap = Math.Max(maxOverlap, overlapCount);
        }

        return maxOverlap;
    }

    private static int OverlapCountAt(int candidateStart, List<(int Start, int End)> bookings)
    {
        var overlapCount = 0;

        foreach (var (start, end) in bookings)
        {
            if (start <= candidateStart && candidateStart < end)
            {
                overlapCount++;
            }
        }

        return overlapCount;
    }

    [Benchmark]
    public int HashMapMergeSortSweep()
    {
        var calendar = new MyCalendarThree();
        var maxOverlap = 0;

        foreach (var (start, end) in _bookings)
        {
            maxOverlap = calendar.Book(start, end);
        }

        return maxOverlap;
    }

    // See MyCalendarIIITests.MyCalendarThree for the full explanation - repeated
    // here rather than shared because TwoSumBenchmarks/
    // NumberOfLongestIncreasingSubsequenceBenchmarks establish this project keeps
    // its own copy of the solution rather than depending on the Tests project.
    private sealed class MyCalendarThree
    {
        private readonly HashMap<int, int> _delta = new();

        public int Book(int start, int end)
        {
            AddDelta(start, 1);
            AddDelta(end, -1);

            var keys = _delta.Keys.ToArray();
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(keys));

            var running = 0;
            var maxOverlap = 0;

            foreach (var key in keys)
            {
                _delta.TryGetValue(key, out var change);
                running += change;
                maxOverlap = Math.Max(maxOverlap, running);
            }

            return maxOverlap;
        }

        private void AddDelta(int point, int amount)
        {
            var current = _delta.TryGetValue(point, out var existing) ? existing : 0;
            _delta.Set(point, current + amount);
        }
    }
}
