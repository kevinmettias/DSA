using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarI;

// LeetCode 729. My Calendar I: Book(start, end) rejects any double booking (two
// events sharing so much as a single point in time) and accepts otherwise -
// half-open [start, end) semantics where touching endpoints do NOT conflict.
// IntervalSet<TKey>'s own overlap rule is CLOSED-interval (touching endpoints DO
// merge/conflict - see its own doc comment, which explicitly calls out My
// Calendar as the semantics it does NOT model and says callers must adjust).
// This repo already has the adjustment idiom for exactly this mismatch, just in
// the opposite direction: DataStreamAsDisjointIntervalsTests (LC 352) encodes a
// single point v as the closed pair (v, v + 1) to gain adjacency-merging.
// Here each half-open event [start, end) is instead encoded as the closed pair
// (start, end - 1), which makes IntervalSet's closed-interval overlap check
// exactly equivalent to half-open overlap for integer endpoints.
public sealed partial class MyCalendarITests
{
    [Fact]
    public void Book_LeetCodeExample_RejectsOnlyTheOverlappingEvent()
    {
        var calendar = new MyCalendarI();

        Assert.True(calendar.Book(10, 20));
        Assert.False(calendar.Book(15, 25));
        Assert.True(calendar.Book(20, 30));
    }

    [Fact]
    public void Book_TouchingEndpoints_BothAccepted()
    {
        var calendar = new MyCalendarI();

        Assert.True(calendar.Book(5, 10));
        Assert.True(calendar.Book(10, 15));
    }

    private sealed class MyCalendarI
    {
        private readonly IntervalSet<int> _bookings = new();

        public bool Book(int start, int end)
        {
            if (_bookings.HasOverlap(start, end - 1))
            {
                return false;
            }

            _bookings.Add(start, end - 1);
            return true;
        }
    }
}
