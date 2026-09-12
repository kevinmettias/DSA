using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.MyCalendarII;

// LeetCode 731. My Calendar II: Book(start, end) allows a double booking (two
// events overlapping) but rejects any booking that would create a TRIPLE
// booking - half-open [start, end) semantics, so touching endpoints never
// count as overlapping. Both strategies use MyCalendarISolution's (LC 729)
// half-open-to-closed (end - 1) encoding wherever they touch IntervalSet<int>,
// whose own overlap rule is closed-interval.
//
// This is a "design" problem: the published interface is a stateful class
// replayed across a sequence of Book calls, so the strategies here are factory
// methods returning that stateful object, the same shape MyCalendarISolution
// uses.
internal static class MyCalendarIISolution
{
    public interface ICalendar
    {
        bool Book(int start, int end);
    }

    // Real answer: two of this repo's own IntervalSet<int> instances - _covered
    // merges every accepted event, so it always reports the region booked at
    // least once; _doubled merges every region already booked exactly twice. A
    // new event is rejected only if it touches _doubled (that would push some
    // point to 3). Otherwise its intersection with every interval _covered
    // already stores becomes new double-booked territory - safe, because
    // passing the _doubled check first guarantees nothing _covered reports
    // here is already at 2. Manually scans IntervalSet's own public Get/Count
    // rather than reaching into private storage.
    public static ICalendar CreateByTwoIntervalSetScan() => new TwoIntervalSetScanCalendar();

    // Baseline: two flat BCL List<(int,int)> - one of every raw booking, one of
    // every double-booked region - both scanned linearly on each call, O(n)
    // per call. Deliberately written without this repo's IntervalSet - it is
    // the arm CreateByTwoIntervalSetScan has to justify itself against.
    public static ICalendar CreateByTwoListScan() => new TwoListScanCalendar();

    private sealed class TwoIntervalSetScanCalendar : ICalendar
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

    private sealed class TwoListScanCalendar : ICalendar
    {
        private readonly List<(int Start, int End)> _bookings = [];
        private readonly List<(int Start, int End)> _doubled = [];

        public bool Book(int start, int end)
        {
            if (_doubled.Any(d => start < d.End && d.Start < end))
            {
                return false;
            }

            foreach (var (bStart, bEnd) in _bookings)
            {
                var overlapStart = Math.Max(start, bStart);
                var overlapEnd = Math.Min(end, bEnd);

                if (overlapStart < overlapEnd)
                {
                    _doubled.Add((overlapStart, overlapEnd));
                }
            }

            _bookings.Add((start, end));
            return true;
        }
    }
}
