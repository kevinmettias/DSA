using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MyCalendarI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MyCalendarISolution's, the same strategies
// MyCalendarITests proves correct. Every synthetic booking request is spread
// across a wide, non-overlapping domain (Stride > EventWidth) so almost every
// Book call succeeds and the stored set grows to the full Length, isolating
// the cost of the overlap CHECK itself. Drain feeds the whole generated event
// sequence through Book() one call at a time - the LeetCode-shaped sequence
// itself, not a batch construction - counting how many were accepted.
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
    public int LinearScan() => Drain(MyCalendarISolution.CreateByLinearScan());

    [Benchmark]
    public int IntervalSetBinarySearch() => Drain(MyCalendarISolution.CreateByIntervalSet());

    private int Drain(MyCalendarISolution.ICalendar calendar)
    {
        var accepted = 0;

        foreach (var (start, end) in _events)
        {
            if (calendar.Book(start, end))
            {
                accepted++;
            }
        }

        return accepted;
    }
}
