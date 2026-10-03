using DSAExperimentation.LeetCode.BookingConcertTicketsInGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BookingConcertTicketsInGroupsSolution's, the same
// classes BookingConcertTicketsInGroupsSolutionTests proves correct. [GlobalSetup] draws
// one fixed stream of gather/scatter calls, so script construction is charged to
// setup rather than to the replay each arm measures. The instance itself is built
// inside each [Benchmark] arm and not hoisted - a Design problem's state is mutated
// by the very calls being measured, so a shared instance would let one run's
// bookings leak into the next (the RangeSumQueryMutableBenchmarks
// fresh-rebuild-per-run precedent).
public class BookingConcertTicketsInGroupsBenchmarks
{
    private const int SeatsPerRow = 50;
    private const int OperationCount = 300;
    private const int RandomSeed = 2286; // LC problem number
    private const int OperationTypeCount = 2;

    // Scatter's two answers, boxed once here so recording one allocates nothing in the replay.
    private static readonly object Scattered = true;
    private static readonly object NotScattered = false;

    private (bool IsGather, int GroupSize, int MaxRow)[] _operations = [];

    // Every call's answer, in replay order: Gather's seating, or Scatter's success. Sized in setup
    // so the replay allocates nothing beyond what the strategy itself returns.
    private object[] _answers = [];

    [Params(200, 2_000)]
    public int RowCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _operations = new (bool IsGather, int GroupSize, int MaxRow)[OperationCount];

        for (var i = 0; i < OperationCount; i++)
        {
            var isGather = random.Next(OperationTypeCount) == 0;
            var groupSize = random.Next(1, SeatsPerRow + 1);
            var maxRow = random.Next(0, RowCount);
            _operations[i] = (isGather, groupSize, maxRow);
        }

        _answers = new object[OperationCount];
    }

    [Benchmark(Baseline = true)]
    public object[] RowScan() => Replay(new BookingConcertTicketsInGroupsSolution.BookMyShowByRowScan(RowCount, SeatsPerRow));

    [Benchmark]
    public object[] SegmentTreeBinarySearch() => Replay(new BookingConcertTicketsInGroupsSolution.BookMyShowBySegmentTreeBinarySearch(RowCount, SeatsPerRow));

    // Returns every call's answer in order, so the JIT cannot eliminate the replay
    // as dead code and the arms are compared on everything they answered.
    private object[] Replay(BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy)
    {
        for (var i = 0; i < _operations.Length; i++)
        {
            var (isGather, groupSize, maxRow) = _operations[i];
            _answers[i] = isGather ? strategy.Gather(groupSize, maxRow) : ScatterAnswer(strategy, groupSize, maxRow);
        }

        return _answers;
    }

    private static object ScatterAnswer(
        BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy, int groupSize, int maxRow)
        => strategy.Scatter(groupSize, maxRow) ? Scattered : NotScattered;
}
