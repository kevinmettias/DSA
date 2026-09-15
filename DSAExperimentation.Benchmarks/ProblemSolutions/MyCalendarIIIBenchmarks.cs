using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MyCalendarIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MyCalendarIIISolution's, the same strategies
// MyCalendarIIITests proves correct. Random bookings feed the textbook brute
// force (rescan every start point ever seen against every booking recorded so
// far) against this repo's own HashMap<int,int> delta sweep + MergeSort.Sort
// re-sort. Bookings never get removed, so the running max returned by the
// final call already equals the max over the whole run, letting both arms
// return one comparable int.
[MemoryDiagnoser]
public class MyCalendarIIIBenchmarks
{
    // LC 732.
    private const int RandomSeed = 732;
    private const int StartRangeMultiplier = 2;
    private const int MaxBookingDuration = 20;

    private (int Start, int End)[] _bookings = [];

    [Params(50, 200)]
    public int Length { get; set; }

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
    public int BruteForceEventRescan() => Drain(MyCalendarIIISolution.CreateByBruteForceEventRescan());

    [Benchmark]
    public int HashMapMergeSortSweep() => Drain(MyCalendarIIISolution.CreateByHashMapMergeSortSweep());

    private int Drain(MyCalendarIIISolution.ICalendar calendar)
    {
        var maxOverlap = 0;

        foreach (var (start, end) in _bookings)
        {
            maxOverlap = calendar.Book(start, end);
        }

        return maxOverlap;
    }
}
