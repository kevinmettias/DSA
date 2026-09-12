using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.IntervalSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RangeModule;

// LeetCode 715. Range Module: half-open [left, right) ranges tracked over the real
// line, via addRange/queryRange/removeRange.
//
// This is a design problem - LeetCode's own shape is a stateful object with three
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution/BinarySearchTreeIteratorSolution use for their own
// design problems.
//
// CreateByIntervalSetBinarySearch composes straight onto IntervalSet<int>.Add -
// unlike DataStreamAsDisjointIntervalsTests (LC 352), no +1 encoding trick is needed
// here, because IntervalSet's closed-interval overlap rule (a <= d && c <= b) already
// treats two half-open ranges stored as raw (Start, End) pairs as touching/merging
// exactly when they share a boundary value, which is precisely the "no gap between
// them" condition half-open range coverage needs. queryRange finds the one interval
// that could contain [left, right) via BinarySearch.UpperBound over a Starts view -
// the same "IRandomAccessSequence witness over an existing structure's own Get" idiom
// IntervalSet.cs's own HasOverlap/Add already use internally (its own doc comment
// names this as a sanctioned reuse, ARCHITECTURE.md §4's ByPriorityOrder precedent) -
// then checks that candidate's End reaches right. removeRange has no counterpart on
// IntervalSet (it exposes no Remove), so it rebuilds: split every stored interval
// that overlaps [left, right) into its surviving fragments and re-Add each one to a
// fresh IntervalSet, composing only Get/Count/Add, never reaching into IntervalSet's
// private storage.
//
// CreateByLinearScan is the textbook baseline this composition has to justify itself
// against: a plain List<(int,int)> of disjoint merged ranges, with addRange merging
// via an O(n) scan and queryRange checking every stored range for full containment,
// O(n) per call, instead of IntervalSet's binary search.
internal static class RangeModuleSolution
{
    public static IRangeModule CreateByIntervalSetBinarySearch() => new IntervalSetBinarySearchRangeModule();

    public static IRangeModule CreateByLinearScan() => new LinearScanRangeModule();

    internal interface IRangeModule
    {
        void AddRange(int left, int right);

        bool QueryRange(int left, int right);

        void RemoveRange(int left, int right);
    }

    // Split one stored interval around a removal window, keeping only the fragments
    // that survive - shared by both strategies since it is plain interval arithmetic,
    // not the representation choice that distinguishes them.
    private static void AddSurvivingFragments(
        (int Start, int End) interval,
        (int Left, int Right) removalRange,
        List<(int Start, int End)> survivors)
    {
        var (start, end) = interval;
        var (left, right) = removalRange;

        if (end <= left || start >= right)
        {
            survivors.Add((start, end));
            return;
        }

        if (start < left)
        {
            survivors.Add((start, left));
        }

        if (right < end)
        {
            survivors.Add((right, end));
        }
    }

    private sealed class IntervalSetBinarySearchRangeModule : IRangeModule
    {
        private IntervalSet<int> _ranges = new();

        public void AddRange(int left, int right) => _ranges.Add(left, right);

        public bool QueryRange(int left, int right)
        {
            var candidate = BinarySearch.UpperBound<int, StartsView>(new StartsView(_ranges), left) - 1;
            return candidate >= 0 && _ranges.Get(candidate).End >= right;
        }

        public void RemoveRange(int left, int right)
        {
            var survivors = new List<(int Start, int End)>();

            for (var i = 0; i < _ranges.Count; i++)
            {
                AddSurvivingFragments(_ranges.Get(i), (left, right), survivors);
            }

            _ranges = new IntervalSet<int>();
            foreach (var (start, end) in survivors)
            {
                _ranges.Add(start, end);
            }
        }

        private readonly struct StartsView(IntervalSet<int> intervals) : IRandomAccessSequence<int>
        {
            public int Length => intervals.Count;

            public int Get(int index) => intervals.Get(index).Start;
        }
    }

    private sealed class LinearScanRangeModule : IRangeModule
    {
        private readonly List<(int Start, int End)> _ranges = [];

        public void AddRange(int left, int right)
        {
            var merged = new List<(int Start, int End)>();
            var newStart = left;
            var newEnd = right;

            foreach (var (start, end) in _ranges)
            {
                if (end < newStart || start > newEnd)
                {
                    merged.Add((start, end));
                }
                else
                {
                    newStart = Math.Min(newStart, start);
                    newEnd = Math.Max(newEnd, end);
                }
            }

            merged.Add((newStart, newEnd));

            _ranges.Clear();
            _ranges.AddRange(merged);
        }

        public bool QueryRange(int left, int right) =>
            _ranges.Any(range => range.Start <= left && right <= range.End);

        public void RemoveRange(int left, int right)
        {
            var survivors = new List<(int Start, int End)>();

            foreach (var range in _ranges)
            {
                AddSurvivingFragments(range, (left, right), survivors);
            }

            _ranges.Clear();
            _ranges.AddRange(survivors);
        }
    }
}
