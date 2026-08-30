using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarII;

// LeetCode 731. My Calendar II: Book(start, end) allows a double booking (two
// events overlapping) but rejects any booking that would create a TRIPLE
// booking. Composes two IntervalSet<int> instances, both using the same
// half-open-to-closed (end - 1) encoding MyCalendarITests (LC 729) establishes:
// _covered merges every accepted event, so it always reports the region booked
// at least once; _doubled merges every region already booked exactly twice.
// A new event is rejected only if it touches _doubled (that would push some
// point to 3). Otherwise, its intersection with every interval _covered already
// stores becomes new double-booked territory - safe, because passing the
// _doubled check first guarantees nothing _covered reports here is already at
// 2. This is a manual scan over IntervalSet's own public Get/Count, the same
// "compose Get/Count directly, never reach into private storage" idiom
// RangeModuleTests.RemoveRange (LC 715) already uses.
public sealed partial class MyCalendarIITests
{
    [Fact]
    public void Book_LeetCodeExample_RejectsOnlyTheTripleBooking()
    {
        var calendar = new MyCalendarII();

        Assert.True(calendar.Book(10, 20));
        Assert.True(calendar.Book(50, 60));
        Assert.True(calendar.Book(10, 40));
        Assert.False(calendar.Book(5, 15));
        Assert.True(calendar.Book(5, 10));
        Assert.True(calendar.Book(25, 55));
    }

    [Fact]
    public void Book_TouchingEndpoints_NeverDoubleBooks()
    {
        var calendar = new MyCalendarII();

        Assert.True(calendar.Book(5, 10));
        Assert.True(calendar.Book(10, 15));
        Assert.True(calendar.Book(0, 5));
    }

    private sealed class MyCalendarII
    {
        private readonly IntervalSet<int> _covered = new();
        private readonly IntervalSet<int> _doubled = new();

        public bool Book(int start, int end)
        {
            var closedEnd = end - 1;

            if (_doubled.HasOverlap(start, closedEnd))
            {
                return false;
            }

            for (var i = 0; i < _covered.Count; i++)
            {
                var (existingStart, existingEnd) = _covered.Get(i);
                var overlapStart = Math.Max(start, existingStart);
                var overlapEnd = Math.Min(closedEnd, existingEnd);

                if (overlapStart <= overlapEnd)
                {
                    _doubled.Add(overlapStart, overlapEnd);
                }
            }

            _covered.Add(start, closedEnd);
            return true;
        }
    }
}
