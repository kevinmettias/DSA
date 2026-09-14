using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.CountIntegersInIntervals;

// LeetCode 2276. Count Integers in Intervals: a stateful set of integers grown by
// add(left, right) - "add every integer in [left, right]" - answering count() with the
// size of the union.
//
// LC 2276 is a "design" problem: the published interface is a stateful class replayed
// across a sequence of add/count calls, not a single pure function, so the strategies
// here are factory methods returning that stateful object - the same shape
// DataStreamAsDisjointIntervalsSolution uses for LC 352's own interval-summary object.
internal static class CountIntegersInIntervalsSolution
{
    public interface ICountIntervals
    {
        void Add(int left, int right);

        int Count();
    }

    // Real answer: Add delegates straight to this repo's own IntervalSet<int>.Add(left,
    // right). The CLOSED-interval semantics IntervalSet already implements (see
    // IntervalSet.cs's own doc comment) match this problem exactly, with no encoding
    // needed - unlike LC 352, which needs the width-1 half-open trick to get adjacency:
    // here two ranges must merge precisely when they share an actual integer VALUE
    // (e.g. [1,3] and [3,5] both contain 3), which is exactly when NOT merging them
    // would double-count that shared integer, and that is already IntervalSet's rule.
    // Add locates its merge point through BinarySearch.LowerBound internally, so a call
    // never costs more than the number of intervals it actually touches.
    //
    // Count() sums (End - Start + 1) across the disjoint intervals currently held rather
    // than maintaining a running total field, so it stays correct no matter how many
    // existing ranges a single Add collapsed behind it.
    public static ICountIntervals CreateByIntervalSetMerge() => new IntervalSetCountIntervals();

    // Baseline: the approach most people reach for first - insert every individual
    // integer of [left, right] into a HashSet<int> and read its Count. O(range width)
    // per add, and O(total distinct integers) in memory. Deliberately BCL only; it is
    // the arm CreateByIntervalSetMerge has to justify itself against.
    public static ICountIntervals CreateByHashSetPerInteger() => new HashSetCountIntervals();

    private sealed class IntervalSetCountIntervals : ICountIntervals
    {
        private readonly IntervalSet<int> _intervals = new();

        public void Add(int left, int right) => _intervals.Add(left, right);

        public int Count()
        {
            var total = 0;

            for (var i = 0; i < _intervals.Count; i++)
            {
                var (start, end) = _intervals.Get(i);
                total += end - start + 1;
            }

            return total;
        }
    }

    private sealed class HashSetCountIntervals : ICountIntervals
    {
        private readonly HashSet<int> _seen = [];

        public void Add(int left, int right)
        {
            for (var value = left; value <= right; value++)
            {
                _seen.Add(value);
            }
        }

        public int Count() => _seen.Count;
    }
}
