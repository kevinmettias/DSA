using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.DataStructures.IntervalSet;

// A sorted, merged set of disjoint intervals over TKey, each stored as a pair
// (Start, End). Add merges any two that overlap or touch (a <= d && c <= b), so
// endpoint-touching pairs like [1,2] and [2,3] merge - no TKey increment
// operation is needed to express "adjacent." That one merge rule serves both
// readings of a pair: for closed intervals [Start, End] (the Merge Intervals /
// Insert Interval family) it is the closed union, and for half-open intervals
// [Start, End) (My Calendar, Range Module) it is exactly the half-open union,
// since [1,2) and [2,3) together cover [1,3). Only the overlap question differs:
// HasOverlap asks it closed, where a shared endpoint conflicts, and
// HasOverlapHalfOpen asks it half-open, where it does not.
//
// Backed by DynamicArray<(TKey,TKey)> kept sorted by Start - composing this
// repo's own already-public Representation type the way DynamicArraySequence
// does (ARCHITECTURE.md §9.4), not a BCL List. Because non-overlap is maintained
// as an invariant (every Add merges through anything it touches), End values
// stay sorted in the same relative order as Start - which is what lets Add and
// both overlap queries locate candidates via IntervalEndsView, a private struct
// implementing Searching's own IRandomAccessSequence<TKey> "as a client" (the same
// sanctioned reuse ByPriorityOrder makes of Heap's IHeapOrder<T>, per
// ARCHITECTURE.md §4), so each reuses BinarySearch's LowerBound or UpperBound
// directly instead of a second, duplicate bisection loop.
//
// The comparer stays a plain runtime IComparer<TKey> - same §9.2 reasoning
// BinarySearch/MergeSort already establish: any TKey, any total order, is a
// valid comparer, an open-ended space no witness could usefully close over.
public sealed class IntervalSet<TKey>
{
    private readonly DynamicArray<(TKey Start, TKey End)> _intervals = new();
    private readonly IComparer<TKey> _comparer;

    public int Count => _intervals.Count;

    public IntervalSet()
        : this(Comparer<TKey>.Default)
    {
    }

    public IntervalSet(IComparer<TKey> comparer) => _comparer = comparer;

    public (TKey Start, TKey End) Get(int index) => _intervals.Get(index);

    // Inserts [start, end], merging with every existing interval it overlaps.
    public void Add(TKey start, TKey end)
    {
        var mergeStart = LowerBoundByEnd(start);
        var merged = MergeOverlapping(mergeStart, start, end);

        for (var i = 0; i < merged.Count; i++)
        {
            _intervals.RemoveAt(mergeStart);
        }

        _intervals.Insert(mergeStart, (merged.Start, merged.End));
    }

    private (TKey Start, TKey End, int Count) MergeOverlapping(int fromIndex, TKey start, TKey end)
    {
        var mergedStart = start;
        var mergedEnd = end;
        var count = 0;

        while (fromIndex + count < _intervals.Count
            && _comparer.Compare(_intervals.Get(fromIndex + count).Start, end) <= 0)
        {
            var (existingStart, existingEnd) = _intervals.Get(fromIndex + count);
            var startsEarlier = _comparer.Compare(existingStart, mergedStart) < 0;
            var endsLater = _comparer.Compare(existingEnd, mergedEnd) > 0;
            mergedStart = startsEarlier ? existingStart : mergedStart;
            mergedEnd = endsLater ? existingEnd : mergedEnd;
            count++;
        }

        return (mergedStart, mergedEnd, count);
    }

    // Whether any existing interval shares a point with [start, end].
    public bool HasOverlap(TKey start, TKey end)
    {
        var candidate = LowerBoundByEnd(start);

        return candidate < _intervals.Count && _comparer.Compare(_intervals.Get(candidate).Start, end) <= 0;
    }

    /// <summary>
    /// Whether any stored interval shares a point with the half-open query, reading every
    /// interval half-open, so one that only touches an endpoint does not overlap.
    /// </summary>
    /// <param name="start">The query's inclusive start.</param>
    /// <param name="end">The query's exclusive end.</param>
    /// <returns><see langword="true"/> when some stored interval overlaps the query.</returns>
    public bool HasOverlapHalfOpen(TKey start, TKey end)
    {
        var candidate = UpperBoundByEnd(start);

        return candidate < _intervals.Count && _comparer.Compare(_intervals.Get(candidate).Start, end) < 0;
    }

    private int LowerBoundByEnd(TKey start)
        => BinarySearch.LowerBound<TKey, IntervalEndsView>(new IntervalEndsView(_intervals), start, _comparer);

    private int UpperBoundByEnd(TKey start)
        => BinarySearch.UpperBound<TKey, IntervalEndsView>(new IntervalEndsView(_intervals), start, _comparer);

    private readonly struct IntervalEndsView(DynamicArray<(TKey Start, TKey End)> intervals) : IRandomAccessSequence<TKey>
    {
        public int Length => intervals.Count;

        public TKey Get(int index) => intervals.Get(index).End;
    }
}
