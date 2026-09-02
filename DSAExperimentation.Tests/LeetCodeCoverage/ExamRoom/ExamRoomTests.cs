using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExamRoom;

// LeetCode 855. Exam Room: occupied seats kept as a sorted DynamicArray<int>,
// the same DynamicArray<(TKey,TKey)>-as-sorted-storage move IntervalSet.cs
// already makes (composing this repo's own Representation type directly
// rather than a bare List<int>). Seat() walks every adjacent occupied pair
// once to find the widest min-distance gap - that same walk already knows
// which index the new seat belongs at, so no separate search is needed
// there. Leave(p) has no such walk to piggyback on, so it locates p the same
// way IntervalSet/MyCalendarI already do: this repo's own
// BinarySearch.LowerBound over a DynamicArraySequence<int> view, O(log n)
// instead of a linear scan.
public sealed partial class ExamRoomTests
{
    [Fact]
    public void Seat_LeetCodeExample_MaximizesDistanceToNearestStudentEachCall()
    {
        var room = new ExamRoom(10);

        Assert.Equal(0, room.Seat());
        Assert.Equal(9, room.Seat());
        Assert.Equal(4, room.Seat());
        Assert.Equal(2, room.Seat());

        room.Leave(4);

        Assert.Equal(5, room.Seat());
    }

    [Fact]
    public void Seat_EmptyRoom_ReturnsSeatZero()
    {
        var room = new ExamRoom(5);

        Assert.Equal(0, room.Seat());
    }

    private sealed class ExamRoom(int seatCount)
    {
        private readonly DynamicArray<int> _occupied = new();

        public int Seat()
        {
            if (_occupied.Count == 0)
            {
                _occupied.Insert(0, 0);
                return 0;
            }

            var best = FindBestGap();
            best = ConsiderEndOfRow(best);

            _occupied.Insert(best.Index, best.Seat);
            return best.Seat;
        }

        private SeatCandidate FindBestGap()
        {
            var best = new SeatCandidate(0, 0, _occupied.Get(0));

            for (var i = 0; i < _occupied.Count - 1; i++)
            {
                var (candidate, distance) = EvaluateGap(i);

                if (distance > best.Distance)
                {
                    best = new SeatCandidate(i + 1, candidate, distance);
                }
            }

            return best;
        }

        private SeatCandidate ConsiderEndOfRow(SeatCandidate best)
        {
            var lastSeat = _occupied.Get(_occupied.Count - 1);
            var endDistance = seatCount - 1 - lastSeat;

            return endDistance > best.Distance
                ? new SeatCandidate(_occupied.Count, seatCount - 1, endDistance)
                : best;
        }

        private readonly record struct SeatCandidate(int Index, int Seat, int Distance);

        public void Leave(int p)
        {
            var index = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(new DynamicArraySequence<int>(_occupied), p);
            _occupied.RemoveAt(index);
        }

        private (int Candidate, int Distance) EvaluateGap(int i)
        {
            var left = _occupied.Get(i);
            var right = _occupied.Get(i + 1);
            var candidate = left + ((right - left) / 2);
            var distance = candidate - left;
            return (candidate, distance);
        }
    }
}
