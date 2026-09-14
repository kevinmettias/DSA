using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MeetingRoomsIII;

// LeetCode 2402. Meeting Rooms III: meetings are held in start-time order, each in
// the lowest-numbered room that is free; when every room is busy the meeting waits
// for the room that frees soonest (lowest room number on a tie) and keeps its
// original duration. Report the room that held the most meetings, lowest index on a
// tie.
//
// Both strategies simulate the same booking sequence and differ only in how they
// answer the two queries the simulation needs - "lowest-numbered free room" and
// "room that frees soonest": a linear scan of a freeAt[] array, or a pair of heaps.
//
// End times are tracked as `long` throughout: a room that is delayed repeatedly
// accumulates every wait, so the running end time can carry past int range even
// though each individual input end time fits in an int.
internal static class MeetingRoomsIIISolution
{
    // No room is free at this meeting's start time - the meeting has to wait.
    private const int NoFreeRoom = -1;

    // The textbook answer: one freeAt timestamp per room, scanned once for the
    // lowest free room and again for the soonest-to-free room when nothing is free -
    // O(meetings * rooms), BCL-only, the arm the composed solution below has to
    // justify itself against.
    public static int MostBookedByLinearScanFreeAt(int roomCount, int[][] meetings)
    {
        var freeAt = new long[roomCount];
        var handled = new int[roomCount];

        foreach (var index in StartOrder(meetings))
        {
            var meeting = meetings[index];
            BookByScan(freeAt, handled, meeting[0], meeting[1]);
        }

        return IndexOfBusiest(handled);
    }

    private static void BookByScan(long[] freeAt, int[] handled, int start, int end)
    {
        var chosen = FirstFreeRoom(freeAt, start);

        if (chosen == NoFreeRoom)
        {
            chosen = SoonestFreeRoom(freeAt);
            freeAt[chosen] += end - start;
        }
        else
        {
            freeAt[chosen] = end;
        }

        handled[chosen]++;
    }

    private static int FirstFreeRoom(long[] freeAt, int start)
    {
        for (var room = 0; room < freeAt.Length; room++)
        {
            if (freeAt[room] <= start)
            {
                return room;
            }
        }

        return NoFreeRoom;
    }

    private static int SoonestFreeRoom(long[] freeAt)
    {
        var soonest = 0;

        for (var room = 1; room < freeAt.Length; room++)
        {
            if (freeAt[room] < freeAt[soonest])
            {
                soonest = room;
            }
        }

        return soonest;
    }

    // Two of this repo's own heaps standing in for the two scans above, the same
    // two-structure shape FindServersThatHandledMostNumberOfRequestsSolution uses for
    // its ring of servers: a Heap<int, MinHeapOrder<int>> free pool whose root is
    // always the lowest free room number, and a
    // Heap<(long End, int Room), MinHeapOrder<(long, int)>> busy pool whose root is
    // always the room that frees soonest - ValueTuple's own IComparable already
    // orders by End first and Room second, which is exactly LeetCode's tie rule.
    // O(meetings * log rooms) instead of the O(meetings * rooms) above.
    public static int MostBookedByTwoHeapPool(int roomCount, int[][] meetings)
    {
        var pool = new RoomPool(roomCount);

        foreach (var index in StartOrder(meetings))
        {
            var meeting = meetings[index];
            pool.Book(meeting[0], meeting[1]);
        }

        return IndexOfBusiest(pool.Handled);
    }

    // LeetCode lists meetings in arbitrary order and the simulation is only correct
    // in start-time order. Start times are distinct, so ordering by start is total
    // and no tie rule is needed here.
    private static int[] StartOrder(int[][] meetings)
        => [.. Enumerable.Range(0, meetings.Length).OrderBy(index => meetings[index][0])];

    // LeetCode reports the lowest-numbered room among those tied for the most
    // meetings, which is what a strict `>` against the incumbent gives.
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

        public int[] Handled { get; }

        public RoomPool(int roomCount)
        {
            Handled = new int[roomCount];

            for (var room = 0; room < roomCount; room++)
            {
                _free.Push(room);
            }
        }

        public void Book(int start, int end)
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
