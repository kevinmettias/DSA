using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.MyCalendarI;

// LeetCode 729. My Calendar I: Book(start, end) rejects any double booking (two
// events sharing so much as a single point in time) and accepts otherwise -
// half-open [start, end) semantics where touching endpoints do NOT conflict.
// IntervalSet<TKey>'s own overlap rule is CLOSED-interval (touching endpoints DO
// merge/conflict - see its own doc comment, which explicitly calls out My
// Calendar as the semantics it does NOT model and says callers must adjust).
// This repo already has the adjustment idiom for exactly this mismatch, just in
// the opposite direction: DataStreamAsDisjointIntervalsSolution (LC 352) encodes
// a single point v as the closed pair (v, v + 1) to gain adjacency-merging.
// Here each half-open event [start, end) is instead encoded as the closed pair
// (start, end - 1), which makes IntervalSet's closed-interval overlap check
// exactly equivalent to half-open overlap for integer endpoints.
//
// LC 729 is a "design" problem: the published interface is a stateful class
// replayed across a sequence of Book calls, not a single pure function, so the
// strategies here are factory methods returning that stateful object - the same
// shape DataStreamAsDisjointIntervalsSolution uses for LC 352.
internal static class MyCalendarISolution
{
    public interface ICalendar
    {
        bool Book(int start, int end);
    }

    // Real answer: this repo's own IntervalSet<int>.HasOverlap/Add, encoding
    // each half-open [start, end) event as the closed pair (start, end - 1) so
    // the overlap check becomes an O(log n) binary search
    // (IntervalSet.HasOverlap's own BinarySearch.LowerBound) instead of a scan.
    public static ICalendar CreateByIntervalSet() => new IntervalSetCalendar();

    // Baseline: a flat BCL List<(int,int)> of accepted bookings, tested against
    // every stored booking with a half-open overlap predicate on every Book
    // call - O(n) per call. Deliberately written without this repo's
    // IntervalSet - it is the arm CreateByIntervalSet has to justify itself
    // against.
    public static ICalendar CreateByLinearScan() => new LinearScanCalendar();

    private sealed class IntervalSetCalendar : ICalendar
    {
        private readonly IntervalSet<int> _bookings = new();

        public bool Book(int start, int end)
        {
            var closedEnd = end - 1;

            if (_bookings.HasOverlap(start, closedEnd))
            {
                return false;
            }

            _bookings.Add(start, closedEnd);
            return true;
        }
    }

    private sealed class LinearScanCalendar : ICalendar
    {
        private readonly List<(int Start, int End)> _bookings = [];

        public bool Book(int start, int end)
        {
            if (_bookings.Any(b => start < b.End && b.Start < end))
            {
                return false;
            }

            _bookings.Add((start, end));
            return true;
        }
    }
}
