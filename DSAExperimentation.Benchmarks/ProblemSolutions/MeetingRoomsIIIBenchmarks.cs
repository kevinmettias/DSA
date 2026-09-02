using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Meeting Rooms III (LC 2402): a linear scan of a freeAt[] array (O(RoomCount) per
// meeting - scan once for the lowest free room, again for the soonest-to-free room
// on delay) vs. this repo's own two heaps - Heap<int, MinHeapOrder<int>> free-room
// pool + Heap<(long,int), MinHeapOrder<(long,int)>> busy-room pool ordered by end
// time then room number (FindServersThatHandledMostNumberOfRequestsBenchmarks' own
// two-structure shape, adapted from a ring of servers to a fixed pool of rooms).
// MeetingCount is generated wide enough relative to RoomCount that rooms routinely
// contend and delay, forcing real pool churn instead of every meeting finding an
// immediately free room.
[MemoryDiagnoser]
public class MeetingRoomsIIIBenchmarks
{
    private const int MeetingsPerRoom = 50;
    private const int MaxDurationMultiplier = 3;

    [Params(20, 80)]
    public int RoomCount;

    private int[] _start = null!;
    private int[] _end = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var meetingCount = RoomCount * MeetingsPerRoom;
        _start = new int[meetingCount];
        _end = new int[meetingCount];

        for (var i = 0; i < meetingCount; i++)
        {
            _start[i] = i;
            _end[i] = i + random.Next(1, (RoomCount * MaxDurationMultiplier) + 1);
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearScanFreeAt()
    {
        var freeAt = new long[RoomCount];
        var handled = new int[RoomCount];

        for (var i = 0; i < _start.Length; i++)
        {
            ProcessLinear(i, freeAt, handled);
        }

        return IndexOfBusiest(handled);
    }

    private void ProcessLinear(int i, long[] freeAt, int[] handled)
    {
        var start = _start[i];
        var end = _end[i];
        var chosen = -1;

        for (var room = 0; room < freeAt.Length; room++)
        {
            if (freeAt[room] <= start)
            {
                chosen = room;
                break;
            }
        }

        if (chosen == -1)
        {
            chosen = 0;

            for (var room = 1; room < freeAt.Length; room++)
            {
                if (freeAt[room] < freeAt[chosen])
                {
                    chosen = room;
                }
            }

            freeAt[chosen] += end - start;
        }
        else
        {
            freeAt[chosen] = end;
        }

        handled[chosen]++;
    }

    [Benchmark]
    public int TwoHeapPool()
    {
        var pool = new RoomPool(RoomCount);

        for (var i = 0; i < _start.Length; i++)
        {
            pool.Process(_start[i], _end[i]);
        }

        return IndexOfBusiest(pool.Handled);
    }

    private static int IndexOfBusiest(int[] handled)
    {
        var busiest = 0;

        for (var room = 1; room < handled.Length; room++)
        {
            if (handled[room] > handled[busiest])
            {
                busiest = room;
            }
        }

        return busiest;
    }

    private sealed class RoomPool
    {
        private readonly Heap<int, MinHeapOrder<int>> _free = new();
        private readonly Heap<(long End, int Room), MinHeapOrder<(long, int)>> _busy = new();

        public RoomPool(int n)
        {
            Handled = new int[n];

            for (var room = 0; room < n; room++)
            {
                _free.Push(room);
            }
        }

        public int[] Handled { get; }

        public void Process(int start, int end)
        {
            while (_busy.TryPeek(out var freed) && freed.End <= start)
            {
                _busy.TryPop(out freed);
                _free.Push(freed.Room);
            }

            if (_free.TryPop(out var room))
            {
                _busy.Push((end, room));
            }
            else
            {
                _busy.TryPop(out var earliest);
                room = earliest.Room;
                _busy.Push((earliest.End + (end - start), room));
            }

            Handled[room]++;
        }
    }
}
