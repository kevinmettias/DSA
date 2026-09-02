using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SeatReservationManager;

// LeetCode 1845. Seat Reservation Manager: this repo's Heap<int,MinHeapOrder
// <int>> holds only released seats (never all n up front), backed by a
// monotonically advancing "next fresh seat" counter - reserve() pops the
// smallest released seat if one exists, otherwise mints the next unused
// number; unreserve() pushes the freed seat back.
public sealed partial class SeatReservationManagerTests
{
    [Fact]
    public void Reserve_And_Unreserve_MatchesLeetCodeExample()
    {
        var manager = new SeatManager();

        Assert.Equal(1, manager.Reserve());
        Assert.Equal(2, manager.Reserve());
        manager.Unreserve(2);
        Assert.Equal(2, manager.Reserve());
        Assert.Equal(3, manager.Reserve());
        Assert.Equal(4, manager.Reserve());
        Assert.Equal(5, manager.Reserve());
        manager.Unreserve(5);
        Assert.Equal(5, manager.Reserve());
        Assert.Equal(6, manager.Reserve());
    }

    private sealed class SeatManager
    {
        private readonly Heap<int, MinHeapOrder<int>> _released = new();
        private int _nextFreshSeat = 1;

        public int Reserve() => _released.TryPop(out var seat) ? seat : _nextFreshSeat++;

        public void Unreserve(int seatNumber) => _released.Push(seatNumber);
    }
}
