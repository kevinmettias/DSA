using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MeetingRoomsIII;

// LeetCode 2402. Meeting Rooms III: this repo's own Heap<T,TOrder> stands in twice,
// mirroring FindServersThatHandledMostNumberOfRequestsTests' two-heap shape -
// Heap<int, MinHeapOrder<int>> as the free-room pool (root is always the lowest
// free room number) and Heap<(long End, int Room), MinHeapOrder<(long,int)>> as the
// busy-room pool ordered by end time then room number on a tie (ValueTuple's own
// IComparable already orders End first, Room second - the exact trick that other
// test already relies on). Delayed end times are tracked as `long` since repeated
// delays can accumulate past int range on the benchmark's larger inputs.
public sealed class MeetingRoomsIIITests
{
    [Fact]
    public void MostBooked_LeetCodeExampleOne_ReturnsLowestIndexOnTie()
    {
        int[][] meetings = [[0, 10], [1, 5], [2, 7], [3, 4]];

        var busiest = MostBooked(n: 2, meetings);

        Assert.Equal(0, busiest);
    }

    [Fact]
    public void MostBooked_LeetCodeExampleTwo_ReturnsRoomWithMostMeetings()
    {
        int[][] meetings = [[1, 20], [2, 10], [3, 5], [4, 9], [6, 8]];

        var busiest = MostBooked(n: 3, meetings);

        Assert.Equal(1, busiest);
    }

    [Fact]
    public void MostBooked_SingleRoomHandlesEveryMeeting_ReturnsThatRoom()
    {
        int[][] meetings = [[0, 5], [10, 15], [20, 25]];

        var busiest = MostBooked(n: 4, meetings);

        Assert.Equal(0, busiest);
    }

    private static int MostBooked(int n, int[][] meetings)
    {
        var pool = new RoomPool(n);

        foreach (var i in Enumerable.Range(0, meetings.Length).OrderBy(i => meetings[i][0]))
        {
            pool.Process(meetings[i][0], meetings[i][1]);
        }

        var busiest = 0;
        for (var room = 1; room < n; room++)
        {
            if (pool.Handled[room] > pool.Handled[busiest])
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
