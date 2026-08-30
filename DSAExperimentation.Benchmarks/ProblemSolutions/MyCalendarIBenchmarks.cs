using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// My Calendar I (LC 729): every synthetic booking request is spread across a
// wide, non-overlapping domain (Stride > EventWidth) so almost every Book call
// succeeds and the stored set grows to the full Length, isolating the cost of
// the overlap CHECK itself. LinearScan keeps a flat List<(int,int)> and tests
// every stored booking against a half-open overlap predicate, O(n) per call.
// IntervalSetBinarySearch instead composes this repo's own
// IntervalSet<int>.HasOverlap/Add, encoding each half-open [start, end) event
// as the closed pair (start, end - 1) - see MyCalendarITests for why that
// encoding is what makes IntervalSet's own closed-interval overlap rule
// equivalent to half-open booking semantics - turning the overlap check into
// an O(log n) binary search (IntervalSet.HasOverlap's own BinarySearch.LowerBound).
[MemoryDiagnoser]
public class MyCalendarIBenchmarks
{
    private const int EventWidth = 10;
    private const int Stride = 30;

    [Params(200, 5_000)]
    public int Length;

    private (int Start, int End)[] _events = null!;

    [GlobalSetup]
    public void Setup()
        => _events = Enumerable.Range(0, Length)
            .Select(i => (Start: i * Stride, End: i * Stride + EventWidth))
            .ToArray();

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var bookings = new List<(int Start, int End)>();
        var accepted = 0;

        foreach (var (start, end) in _events)
        {
            if (bookings.Any(b => start < b.End && b.Start < end))
            {
                continue;
            }

            bookings.Add((start, end));
            accepted++;
        }

        return accepted;
    }

    [Benchmark]
    public int IntervalSetBinarySearch()
    {
        var bookings = new IntervalSet<int>();
        var accepted = 0;

        foreach (var (start, end) in _events)
        {
            var closedEnd = end - 1;
            if (bookings.HasOverlap(start, closedEnd))
            {
                continue;
            }

            bookings.Add(start, closedEnd);
            accepted++;
        }

        return accepted;
    }
}
