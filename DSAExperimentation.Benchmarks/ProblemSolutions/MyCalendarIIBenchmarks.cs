using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// My Calendar II (LC 731): a sliding-window event stream (event i =
// [i*Stride, i*Stride+EventWidth), Stride < EventWidth) where every event
// double-books with its immediate predecessor only, never a triple.
// TwoListScan is the classic solution - a flat List<(int,int)> of every raw
// booking plus a flat List<(int,int)> of every double-booked region, both
// scanned linearly on each call, O(n) per call. TwoIntervalSetScan instead
// composes two of this repo's own IntervalSet<int> instances - one tracking
// merged coverage, one tracking merged double-booked territory (see
// MyCalendarIITests for the full reasoning and the half-open-to-closed
// (end - 1) encoding both use). Because this event stream keeps every accepted
// booking touching the one before it, the covered set stays merged into a
// single interval throughout, so the scan that replaces TwoListScan's O(n)
// bookings walk collapses to O(1), and the triple-booking guard becomes an
// O(log n) binary search (IntervalSet.HasOverlap) instead of a second linear scan.
[MemoryDiagnoser]
public class MyCalendarIIBenchmarks
{
    private const int EventWidth = 10;
    private const int Stride = 5;

    [Params(200, 5_000)]
    public int Length;

    private (int Start, int End)[] _events = null!;

    [GlobalSetup]
    public void Setup()
        => _events = Enumerable.Range(0, Length)
            .Select(i => (Start: i * Stride, End: i * Stride + EventWidth))
            .ToArray();

    [Benchmark(Baseline = true)]
    public int TwoListScan()
    {
        var bookings = new List<(int Start, int End)>();
        var doubles = new List<(int Start, int End)>();
        var accepted = 0;

        foreach (var evt in _events)
        {
            if (TryAcceptBookingByListScan(evt, bookings, doubles))
            {
                accepted++;
            }
        }

        return accepted;
    }

    private static bool TryAcceptBookingByListScan(
        (int Start, int End) evt, List<(int Start, int End)> bookings, List<(int Start, int End)> doubles)
    {
        var (start, end) = evt;

        if (doubles.Any(d => start < d.End && d.Start < end))
        {
            return false;
        }

        foreach (var (bStart, bEnd) in bookings)
        {
            var overlapStart = Math.Max(start, bStart);
            var overlapEnd = Math.Min(end, bEnd);
            if (overlapStart < overlapEnd)
            {
                doubles.Add((overlapStart, overlapEnd));
            }
        }

        bookings.Add((start, end));
        return true;
    }

    [Benchmark]
    public int TwoIntervalSetScan()
    {
        var covered = new IntervalSet<int>();
        var doubled = new IntervalSet<int>();
        var accepted = 0;

        foreach (var evt in _events)
        {
            if (TryAcceptBookingByIntervalSetScan(evt, covered, doubled))
            {
                accepted++;
            }
        }

        return accepted;
    }

    private static bool TryAcceptBookingByIntervalSetScan(
        (int Start, int End) evt, IntervalSet<int> covered, IntervalSet<int> doubled)
    {
        var (start, end) = evt;
        var closedEnd = end - 1;

        if (doubled.HasOverlap(start, closedEnd))
        {
            return false;
        }

        for (var i = 0; i < covered.Count; i++)
        {
            var (existingStart, existingEnd) = covered.Get(i);
            var overlapStart = Math.Max(start, existingStart);
            var overlapEnd = Math.Min(closedEnd, existingEnd);

            if (overlapStart <= overlapEnd)
            {
                doubled.Add(overlapStart, overlapEnd);
            }
        }

        covered.Add(start, closedEnd);
        return true;
    }
}
