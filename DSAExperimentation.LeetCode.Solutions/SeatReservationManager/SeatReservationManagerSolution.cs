using DSAExperimentation.Mechanisms.SmallestFreeAllocator;

namespace DSAExperimentation.LeetCode.Solutions.SeatReservationManager;

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
// array every reserve has to scan from seat 1, or this repo's SmallestFreeAllocator,
// which holds only the seats that were actually given back (ARCHITECTURE.md §17.6b).
[LeetCodeQuestion("seat-reservation-manager", LeetCodeAnswerOrder.AsWritten)]
internal static class SeatReservationManagerSolution
{
    // Each strategy as LeetCode's constructor makes it, so the catalog harness can make the object its call
    // scripts run on (ARCHITECTURE 17.13).
    public static ISeatManager CreateByLinearScanArray(int seatCount) => new SeatManagerByLinearScanArray(seatCount);

    // LeetCode's constructor gives the seat count, which the allocator needs no bound from: it hands out seats
    // from 1 upward as they are asked for.
    public static ISeatManager CreateBySmallestFreeAllocator(int seatCount) => new SeatManagerBySmallestFreeAllocator();

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
    // this repo's SmallestFreeAllocator.
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

    // The composed answer: this repo's own SmallestFreeAllocator, the whole problem
    // as a mechanism. It holds only the released seats, in a min-heap in front of a
    // counter for the seats never issued, so the smallest free seat is either the
    // heap's root or the counter's next, and nothing is stored per seat until a seat
    // is handed back. Seats are numbered from 1.
    //
    // LeetCode's constructor takes the seat count; this strategy is the one that
    // does not need it, which is the point of the lazy-release shape.
    internal sealed class SeatManagerBySmallestFreeAllocator : ISeatManager
    {
        private const int FirstSeat = 1;

        private readonly SmallestFreeAllocator _seats = new(FirstSeat);

        public int Reserve() => _seats.Allocate();

        // LeetCode only unreserves a reserved seat, so the allocator never turns one away.
        public void Unreserve(int seatNumber) => _ = _seats.TryRelease(seatNumber);
    }
}
