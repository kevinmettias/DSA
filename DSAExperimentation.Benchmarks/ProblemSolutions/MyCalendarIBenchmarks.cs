using DSAExperimentation.LeetCode.MyCalendarI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MyCalendarISolution's, the same strategies
// MyCalendarISolutionTests proves correct. Every synthetic booking request is spread
// across a wide, non-overlapping domain (Stride > EventWidth) so almost every
// Book call succeeds and the stored set grows to the full Length, isolating
// the cost of the overlap CHECK itself. Drain feeds the whole generated event
// sequence through Book() one call at a time - the LeetCode-shaped sequence
// itself, not a batch construction - returning every Book answer in order.
public class MyCalendarIBenchmarks
{
    private const int EventWidth = 10;
    private const int Stride = 30;

    private (int Start, int End)[] _events = [];

    private bool[] _booked = [];

    [Params(200, 5_000)]
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
    public bool[] LinearScan() => Drain(MyCalendarISolution.CreateByLinearScan());

    [Benchmark]
    public bool[] IntervalSetBinarySearch() => Drain(MyCalendarISolution.CreateByIntervalSet());

    private bool[] Drain(MyCalendarISolution.ICalendar calendar)
    {
        for (var i = 0; i < _events.Length; i++)
        {
            _booked[i] = calendar.Book(_events[i].Start, _events[i].End);
        }

        return _booked;
    }
}
