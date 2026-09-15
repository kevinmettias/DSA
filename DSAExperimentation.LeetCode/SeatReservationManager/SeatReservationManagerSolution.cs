using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.SeatReservationManager;

// LeetCode 1845. Seat Reservation Manager: n seats numbered 1 to n, all free to
// begin with; reserve() takes the smallest-numbered free seat and returns it, and
// unreserve(seatNumber) hands one back.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared ISeatManager surface below, the same shape
// DesignAuthenticationManagerSolution uses for its own instance-API problem
// (LC 1797).
//
// The whole problem is "where does the smallest free seat come from": an occupancy
// array every reserve has to scan from seat 1, or a min-heap holding only the seats
// that were actually given back.
internal static class SeatReservationManagerSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ISeatManager
    {
        int Reserve();

        void Unreserve(int seatNumber);
    }

    // The textbook baseline this composition has to justify itself against: one
    // bool per seat and a scan from seat 1 on every reserve, so the walk past the
    // already-reserved prefix grows with each call. Deliberately written without
    // this repo's Heap<Element,TOrder>.
    internal sealed class SeatManagerByLinearScanArray(int seatCount) : ISeatManager
    {
        private const string NoSeatsLeftMessage = "No seats left.";

        private readonly bool[] _reserved = new bool[seatCount + 1];

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

    // The composed answer: this repo's own Heap<int,MinHeapOrder<int>> holds only
    // the released seats, behind a counter that mints the next never-issued seat
    // when the heap is empty. The smallest free seat is therefore either the
    // smallest released one - which is exactly what a min-heap's root is - or the
    // frontier, so nothing is stored per seat until a seat is handed back.
    //
    // LeetCode's constructor takes the seat count; this strategy is the one that
    // does not need it, which is the point of the lazy-release shape.
    internal sealed class SeatManagerByReleasedSeatHeap : ISeatManager
    {
        private readonly Heap<int, MinHeapOrder<int>> _released = new();
        private int _nextFreshSeat = 1;

        public int Reserve() => _released.TryPop(out var seat) ? seat : MintNextSeat();

        // The frontier answers with the next never-issued seat number and steps on.
        private int MintNextSeat() => _nextFreshSeat++;

        public void Unreserve(int seatNumber) => _released.Push(seatNumber);
    }
}
