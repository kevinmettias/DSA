using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.DataStreamAsDisjointIntervals;

// LeetCode 352. Data Stream as Disjoint Intervals: addNum needs adjacent integers (a gap of
// exactly 1, e.g. 1 and 2) to merge into one summarized range - a stronger "touching" rule
// than IntervalSet<TKey>'s own closed-interval-overlap merge, which only merges intervals
// that share an actual boundary VALUE (see IntervalSet.cs's own doc comment: it deliberately
// carries no TKey increment operation to express "adjacent" for an arbitrary TKey). Because
// TKey is concretely int here, CreateByIntervalSetMerge still gets adjacency-merging for free
// by encoding each seen value v as the width-1 half-open range [v, v+1), stored as the closed
// pair (v, v+1): two originally-adjacent values v and v+1 then become (v, v+1) and (v+1, v+2),
// which DO share the boundary value v+1 and merge under IntervalSet's existing overlap rule.
// GetIntervals reverses the encoding by reporting (Start, End-1).
//
// LC 352 is a "design" problem: the published interface is a stateful class replayed across a
// sequence of addNum/getIntervals calls, not a single pure function, so the strategies here are
// factory methods returning that stateful object - the same shape BinarySearchTreeIteratorSolution
// uses for LC 173.
internal static class DataStreamAsDisjointIntervalsSolution
{
    public interface ISummaryRanges
    {
        void AddNum(int value);

        IReadOnlyList<(int Start, int End)> GetIntervals();
    }

    // Real answer: this repo's own IntervalSet<int>.Add, encoding each value as a width-1
    // half-open range so adjacent integers merge through IntervalSet's shared-boundary rule.
    // Each addNum is one Add call, which locates its merge point via BinarySearch.LowerBound
    // internally instead of rescanning everything seen so far.
    public static ISummaryRanges CreateByIntervalSetMerge() => new IntervalSetSummaryRanges();

    // Baseline: track every raw value seen and re-derive the disjoint-interval summary from
    // scratch (sort + rescan for consecutive runs) on every addNum - O(n log n) per call.
    // Deliberately BCL only, the arm CreateByIntervalSetMerge has to justify itself against.
    public static ISummaryRanges CreateByFullRebuildEachCall() => new FullRebuildSummaryRanges();

    private sealed class IntervalSetSummaryRanges : ISummaryRanges
    {
        private readonly IntervalSet<int> _intervals = new();

        public void AddNum(int value) => _intervals.Add(value, value + 1);

        public IReadOnlyList<(int Start, int End)> GetIntervals()
            => Enumerable.Range(0, _intervals.Count)
                .Select(_intervals.Get)
                .Select(interval => (interval.Start, interval.End - 1))
                .ToList();
    }

    private sealed class FullRebuildSummaryRanges : ISummaryRanges
    {
        private readonly List<int> _values = [];

        public void AddNum(int value) => _values.Add(value);

        public IReadOnlyList<(int Start, int End)> GetIntervals()
        {
            var sorted = _values.Distinct().OrderBy(value => value).ToList();
            var result = new List<(int Start, int End)>();

            foreach (var value in sorted)
            {
                if (result.Count > 0 && result[^1].End + 1 == value)
                {
                    result[^1] = (result[^1].Start, value);
                }
                else
                {
                    result.Add((value, value));
                }
            }

            return result;
        }
    }
}
