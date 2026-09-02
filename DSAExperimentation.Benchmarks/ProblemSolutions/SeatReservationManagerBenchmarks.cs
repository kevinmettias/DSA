using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Seat Reservation Manager (LC 1845): a naive bool[]-scan-for-smallest-free-
// seat baseline vs. this repo's Heap<int,MinHeapOrder<int>> holding only
// released seats behind a monotonic "next fresh seat" counter. Both replay
// the identical script - OperationCount reserves, then unreserving every
// third issued seat, then OperationCount more reserves - so the linear
// scanner is forced to walk past a growing prefix of already-reserved seats
// on every call instead of an artificially seat-light workload.
[MemoryDiagnoser]
public class SeatReservationManagerBenchmarks
{
    private const int UnreserveStride = 3;
    private const int ReservePassCount = 2; // Reserve is called OperationCount times in two separate passes

    [Params(200, 5_000)]
    public int OperationCount;

    private int[] _unreserveTargets = null!;

    [GlobalSetup]
    public void Setup()
    {
        _unreserveTargets = Enumerable.Range(1, OperationCount / UnreserveStride)
            .Select(i => i * UnreserveStride)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanManager()
    {
        var manager = new LinearScanSeatManager(OperationCount * ReservePassCount + 1);
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

    [Benchmark]
    public int HeapLazyReleaseManager()
    {
        var manager = new HeapSeatManager();
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

    private sealed class LinearScanSeatManager(int capacity)
    {
        private const string NoSeatsLeftMessage = "No seats left.";

        private readonly bool[] _reserved = new bool[capacity + 1];

        public int Reserve()
        {
            for (var seat = 1; seat < _reserved.Length; seat++)
            {
                if (!_reserved[seat])
                {
                    _reserved[seat] = true;
                    return seat;
                }
            }

            throw new InvalidOperationException(NoSeatsLeftMessage);
        }

        public void Unreserve(int seatNumber) => _reserved[seatNumber] = false;
    }

    private sealed class HeapSeatManager
    {
        private readonly Heap<int, MinHeapOrder<int>> _released = new();
        private int _nextFreshSeat = 1;

        public int Reserve() => _released.TryPop(out var seat) ? seat : _nextFreshSeat++;

        public void Unreserve(int seatNumber) => _released.Push(seatNumber);
    }
}
