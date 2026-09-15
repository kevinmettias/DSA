using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SeatReservationManager;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SeatReservationManagerSolution's, the same classes
// SeatReservationManagerTests proves correct - the naive bool[]-scan-for-the-
// smallest-free-seat manager against this repo's Heap<int,MinHeapOrder<int>>
// holding only released seats behind a monotonic "next fresh seat" counter. Both
// replay the identical script - OperationCount reserves, then unreserving every
// third issued seat, then OperationCount more reserves - so the linear scanner is
// forced to walk past a growing prefix of already-reserved seats on every call
// instead of an artificially seat-light workload. [GlobalSetup] materializes the
// unreserve targets so building that list is not charged to either arm.
[MemoryDiagnoser]
public class SeatReservationManagerBenchmarks
{
    private const int UnreserveStride = 3;
    private const int ReservePassCount = 2; private int[] _unreserveTargets = [];

    // Reserve is called OperationCount times in two separate passes

    [Params(200, 5_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _unreserveTargets = Enumerable.Range(1, OperationCount / UnreserveStride)
            .Select(i => i * UnreserveStride)
            .ToArray();

    // The scan baseline is the arm that needs a seat count, and it needs one large
    // enough for every seat both passes issue.
    [Benchmark(Baseline = true)]
    public int LinearScanArray() =>
        Replay(new SeatReservationManagerSolution.SeatManagerByLinearScanArray(OperationCount * ReservePassCount + 1));

    [Benchmark]
    public int ReleasedSeatHeap() => Replay(new SeatReservationManagerSolution.SeatManagerByReleasedSeatHeap());

    private int Replay(SeatReservationManagerSolution.ISeatManager manager)
    {
        var last = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            last = manager.Reserve();
        }

        foreach (var seat in _unreserveTargets)
        {
            manager.Unreserve(seat);
        }

        for (var i = 0; i < OperationCount; i++)
        {
            last = manager.Reserve();
        }

        return last;
    }
}
