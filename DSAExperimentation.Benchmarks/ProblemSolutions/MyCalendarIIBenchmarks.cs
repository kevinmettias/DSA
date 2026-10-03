using DSAExperimentation.LeetCode.MyCalendarII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MyCalendarIISolution's, the same strategies
// MyCalendarIISolutionTests proves correct. A sliding-window event stream (event i =
// [i*Stride, i*Stride+EventWidth), Stride < EventWidth) where every event
// double-books with its immediate predecessor only, never a triple. Drain
// feeds the whole generated event sequence through Book() one call at a
// time - the LeetCode-shaped sequence itself, not a batch construction -
// returning every Book answer in order. Length stops at LC 731's own bound of 1,000
// calls to Book.
public class MyCalendarIIBenchmarks
{
    private const int EventWidth = 10;
    private const int Stride = 5;

    private (int Start, int End)[] _events = [];

    private bool[] _booked = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _events = Enumerable.Range(0, Length)
            .Select(i => (Start: i * Stride, End: i * Stride + EventWidth))
            .ToArray();
        _booked = new bool[_events.Length];
    }

    [Benchmark(Baseline = true)]
    public bool[] TwoListScan() => Drain(MyCalendarIISolution.CreateByTwoListScan());

    [Benchmark]
    public bool[] TwoIntervalSetScan() => Drain(MyCalendarIISolution.CreateByTwoIntervalSetScan());

    private bool[] Drain(MyCalendarIISolution.ICalendar calendar)
    {
        for (var i = 0; i < _events.Length; i++)
        {
            _booked[i] = calendar.Book(_events[i].Start, _events[i].End);
        }

        return _booked;
    }
}
