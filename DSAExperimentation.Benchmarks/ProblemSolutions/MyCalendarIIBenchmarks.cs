using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MyCalendarII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MyCalendarIISolution's, the same strategies
// MyCalendarIITests proves correct. A sliding-window event stream (event i =
// [i*Stride, i*Stride+EventWidth), Stride < EventWidth) where every event
// double-books with its immediate predecessor only, never a triple. Drain
// feeds the whole generated event sequence through Book() one call at a
// time - the LeetCode-shaped sequence itself, not a batch construction -
// counting how many were accepted.
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
    public int TwoListScan() => Drain(MyCalendarIISolution.CreateByTwoListScan());

    [Benchmark]
    public int TwoIntervalSetScan() => Drain(MyCalendarIISolution.CreateByTwoIntervalSetScan());

    private int Drain(MyCalendarIISolution.ICalendar calendar)
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
