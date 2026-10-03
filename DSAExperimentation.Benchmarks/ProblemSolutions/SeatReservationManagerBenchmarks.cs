using DSAExperimentation.LeetCode.SeatReservationManager;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SeatReservationManagerSolution's, the same classes
// SeatReservationManagerSolutionTests proves correct - the naive bool[]-scan-for-the-
// smallest-free-seat manager against this repo's Heap<int,MinHeapOrder<int>>
// holding only released seats behind a monotonic "next fresh seat" counter. Both
// replay the identical script - OperationCount reserves, then unreserving every
// third issued seat, then OperationCount more reserves - so the linear scanner is
// forced to walk past a growing prefix of already-reserved seats on every call
// instead of an artificially seat-light workload. [GlobalSetup] materializes the
// unreserve targets so building that list is not charged to either arm. Each arm
// returns every seat Reserve handed out, in call order.
public class SeatReservationManagerBenchmarks
{
    private const int UnreserveStride = 3;

    // Reserve is called OperationCount times in two separate passes
    private const int ReservePassCount = 2;

    private int[] _unreserveTargets = [];

    private int[] _reserved = [];

    [Params(200, 5_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _unreserveTargets = Enumerable.Range(1, OperationCount / UnreserveStride)
            .Select(i => i * UnreserveStride)
            .ToArray();
        _reserved = new int[OperationCount * ReservePassCount];
    }

    // The scan baseline is the arm that needs a seat count, and it needs one large
    // enough for every seat both passes issue.
    [Benchmark(Baseline = true)]
    public int[] LinearScanArray() =>
        Replay(new SeatReservationManagerSolution.SeatManagerByLinearScanArray(OperationCount * ReservePassCount + 1));

    [Benchmark]
    public int[] ReleasedSeatHeap() => Replay(new SeatReservationManagerSolution.SeatManagerByReleasedSeatHeap());

    private int[] Replay(SeatReservationManagerSolution.ISeatManager manager)
    {
        var reserved = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            _reserved[reserved++] = manager.Reserve();
        }

        foreach (var seat in _unreserveTargets)
        {
            manager.Unreserve(seat);
        }

        for (var i = 0; i < OperationCount; i++)
        {
            _reserved[reserved++] = manager.Reserve();
        }

        return _reserved;
    }
}
