using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ExamRoom;

// LeetCode 855. Exam Room: n seats in a row; Seat() puts the next student in the
// seat maximizing the distance to the closest already-seated student (lowest seat
// number wins ties, and seat 0 when the room is empty), Leave(p) frees seat p.
//
// LC 855 is a "design" problem: the published interface is a stateful object replayed
// across a call script, not a single pure function, so "every strategy for the
// problem" (ARCHITECTURE.md §17.3) takes the form of two classes behind the shared
// IExamRoom surface below - the same shape MyCalendarISolution and
// DesignCircularQueueSolution use for their own instance-API problems.
//
// Both strategies keep the occupied seats SORTED and share the identical Seat() gap
// scan (BestAvailableSeat): one pass over adjacent occupied pairs finds the widest
// min-distance gap, and that same pass already knows the insertion index, so Seat()
// needs no search of its own. They differ only in Leave(p), which has no such walk to
// piggyback on:
//
//   - ByLinearScanList is the textbook baseline - a plain, sorted BCL List<int> whose
//     List<T>.Remove does its own linear scan to locate p, O(n) per call.
//   - ByBinarySearchDynamicArray composes this repo's own DynamicArray<int> as the
//     sorted storage (the same DynamicArray-as-sorted-storage move IntervalSet.cs
//     already makes) plus BinarySearch.LowerBound over a DynamicArraySequence<int>
//     view (IntervalSet/MyCalendarI precedent), locating p in O(log n) before the same
//     O(n) shift every array-backed removal pays either way.
internal static class ExamRoomSolution
{
    // Splits a left/right occupied-seat pair to find the midpoint candidate seat.
    private const int MidpointDivisor = 2;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IExamRoom
    {
        int Seat();

        void Leave(int p);
    }

    // Baseline: sorted BCL List<int>, Leave(p) via List<T>.Remove's own linear scan.
    // Deliberately written without this repo's primitives - it is the arm the composed
    // strategy below has to justify itself against.
    public static IExamRoom CreateByLinearScanList(int seatCount) => new LinearScanListRoom(seatCount);

    // This repo's own DynamicArray<int> + BinarySearch.LowerBound to locate the
    // departing seat in O(log n).
    public static IExamRoom CreateByBinarySearchDynamicArray(int seatCount)
        => new BinarySearchDynamicArrayRoom(seatCount);

    private sealed class LinearScanListRoom(int seatCount) : IExamRoom
    {
        private readonly List<int> _occupied = [];

        public int Seat()
        {
            if (_occupied.Count == 0)
            {
                _occupied.Insert(0, 0);
                return 0;
            }

            var best = BestAvailableSeat(seatCount, _occupied.Count, i => _occupied[i]);
            _occupied.Insert(best.Index, best.Seat);
            return best.Seat;
        }

        public void Leave(int p) => _occupied.Remove(p);
    }

    private sealed class BinarySearchDynamicArrayRoom(int seatCount) : IExamRoom
    {
        private readonly DynamicArray<int> _occupied = new();

        public int Seat()
        {
            if (_occupied.Count == 0)
            {
                _occupied.Insert(0, 0);
                return 0;
            }

            var best = BestAvailableSeat(seatCount, _occupied.Count, _occupied.Get);
            _occupied.Insert(best.Index, best.Seat);
            return best.Seat;
        }

        public void Leave(int p)
        {
            var index = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(
                new DynamicArraySequence<int>(_occupied), p);

            _occupied.RemoveAt(index);
        }
    }

    // The gap scan both strategies share, expressed over an index accessor so the
    // baseline's List<int> and the composed strategy's DynamicArray<int> feed it
    // unchanged - Seat() itself is not what the two differ in.
    private static SeatCandidate BestAvailableSeat(int seatCount, int count, Func<int, int> get)
    {
        var best = new SeatCandidate(0, 0, get(0));

        for (var i = 0; i < count - 1; i++)
        {
            best = ConsiderGapSeat(get, i, best);
        }

        var lastSeat = get(count - 1);
        var endDistance = seatCount - 1 - lastSeat;

        return endDistance > best.Distance
            ? new SeatCandidate(count, seatCount - 1, endDistance)
            : best;
    }

    private static SeatCandidate ConsiderGapSeat(Func<int, int> get, int i, SeatCandidate best)
    {
        var left = get(i);
        var right = get(i + 1);
        var candidate = left + ((right - left) / MidpointDivisor);
        var distance = candidate - left;

        return distance > best.Distance ? new SeatCandidate(i + 1, candidate, distance) : best;
    }

    private readonly record struct SeatCandidate(int Index, int Seat, int Distance);
}
