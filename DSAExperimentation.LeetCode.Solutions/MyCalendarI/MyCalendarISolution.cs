using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.Solutions.MyCalendarI;

// LeetCode 729. My Calendar I: Book(start, end) rejects any double booking (two
// events sharing so much as a single point in time) and accepts otherwise -
// half-open [start, end) semantics where touching endpoints do NOT conflict.
// IntervalSet<TKey> holds the raw half-open bookings: its merge of touching pairs
// is exactly their half-open union, and HasOverlapHalfOpen asks the half-open
// question, so no endpoint needs re-encoding.
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

    // Real answer: this repo's own IntervalSet<int>.HasOverlapHalfOpen/Add over the
    // raw half-open bookings, so the overlap check is an O(log n) binary search
    // (IntervalSet's own BinarySearch.UpperBound) instead of a scan.
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
            if (_bookings.HasOverlapHalfOpen(start, end))
            {
                return false;
            }

            _bookings.Add(start, end);
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
