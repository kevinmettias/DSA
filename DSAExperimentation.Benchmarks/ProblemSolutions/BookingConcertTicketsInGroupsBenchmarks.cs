using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BookingConcertTicketsInGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BookingConcertTicketsInGroupsSolution's, the same
// classes BookingConcertTicketsInGroupsTests proves correct. [GlobalSetup] draws
// one fixed stream of gather/scatter calls, so script construction is charged to
// setup rather than to the replay each arm measures. The instance itself is built
// inside each [Benchmark] arm and not hoisted - a Design problem's state is mutated
// by the very calls being measured, so a shared instance would let one run's
// bookings leak into the next (the RangeSumQueryMutableBenchmarks
// fresh-rebuild-per-run precedent).
[MemoryDiagnoser]
public class BookingConcertTicketsInGroupsBenchmarks
{
    private const int SeatsPerRow = 50;
    private const int OperationCount = 300;
    private const int RandomSeed = 2286; // LC problem number
    private const int OperationTypeCount = 2;

    private (bool IsGather, int K, int MaxRow)[] _operations = [];

    [Params(200, 2_000)]
    public int RowCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _operations = new (bool IsGather, int K, int MaxRow)[OperationCount];

        for (var i = 0; i < OperationCount; i++)
        {
            var isGather = random.Next(OperationTypeCount) == 0;
            var k = random.Next(1, SeatsPerRow + 1);
            var maxRow = random.Next(0, RowCount);
            _operations[i] = (isGather, k, maxRow);
        }
    }

    [Benchmark(Baseline = true)]
    public long RowScan() => Replay(new BookingConcertTicketsInGroupsSolution.BookMyShowByRowScan(RowCount, SeatsPerRow));

    [Benchmark]
    public long SegmentTreeBinarySearch() => Replay(new BookingConcertTicketsInGroupsSolution.BookMyShowBySegmentTreeBinarySearch(RowCount, SeatsPerRow));

    // Folds every answer into a checksum rather than discarding it, so the JIT
    // cannot eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape DesignTaskManagerBenchmarks already follows.
    private long Replay(BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy)
    {
        var checksum = 0L;

        foreach (var (isGather, k, maxRow) in _operations)
        {
            checksum += isGather ? GatherChecksum(strategy, k, maxRow) : ScatterChecksum(strategy, k, maxRow);
        }

        return checksum;
    }

    private static long GatherChecksum(BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy, int k, int maxRow)
    {
        var seating = strategy.Gather(k, maxRow);

        return seating.Length == 0 ? 0 : SeatPairSum(seating);
    }

    private static long SeatPairSum(int[] seating) => seating[0] + seating[1];

    private static long ScatterChecksum(BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy, int k, int maxRow)
        => strategy.Scatter(k, maxRow) ? 1 : 0;
}
