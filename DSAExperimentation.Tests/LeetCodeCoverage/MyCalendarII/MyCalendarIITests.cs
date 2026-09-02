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

        AssertBook(calendar, 10, 20, expectedAccepted: true);
        AssertBook(calendar, 50, 60, expectedAccepted: true);
        AssertBook(calendar, 10, 40, expectedAccepted: true);
        AssertBook(calendar, 5, 15, expectedAccepted: false);
        AssertBook(calendar, 5, 10, expectedAccepted: true);
        AssertBook(calendar, 25, 55, expectedAccepted: true);
    }

    [Fact]
    public void Book_TouchingEndpoints_NeverDoubleBooks()
    {
        var calendar = new MyCalendarII();

        AssertBook(calendar, 5, 10, expectedAccepted: true);
        AssertBook(calendar, 10, 15, expectedAccepted: true);
        AssertBook(calendar, 0, 5, expectedAccepted: true);
    }

    private static void AssertBook(MyCalendarII calendar, int start, int end, bool expectedAccepted)
    {
        var actual = calendar.Book(start, end);
        Assert.Equal(expectedAccepted, actual);
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
