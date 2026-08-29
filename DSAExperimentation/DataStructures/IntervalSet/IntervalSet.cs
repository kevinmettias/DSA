using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.DataStructures.IntervalSet;

// A sorted, merged set of disjoint, CLOSED intervals [Start, End] over TKey. Two
// intervals merge whenever they overlap under the standard closed-interval
// condition (a <= d && c <= b), which already treats endpoint-touching intervals
// like [1,2] and [2,3] as overlapping - no TKey increment operation is needed to
// express "adjacent." This is the Merge Intervals / Insert Interval family's
// semantics, NOT half-open [Start, End) calendar-booking semantics (My Calendar
// I/II/III), where a shared endpoint deliberately does NOT conflict - this type
// does not model that distinction and callers needing it must adjust accordingly.
//
// Backed by DynamicArray<(TKey,TKey)> kept sorted by Start - composing this
// repo's own already-public Representation type the way DynamicArraySequence
// does (ARCHITECTURE.md §9.4), not a BCL List. Because non-overlap is maintained
// as an invariant (every Add merges through anything it touches), End values
// stay sorted in the same relative order as Start - which is what lets Add/
// HasOverlap locate candidates via IntervalEndsView, a private struct implementing
// Searching's own IRandomAccessSequence<TKey> "as a client" (the same sanctioned
// reuse ByPriorityOrder makes of Heap's IHeapOrder<T>, per ARCHITECTURE.md §4),
// so both operations reuse BinarySearch.LowerBound directly instead of a second,
// duplicate bisection loop.
//
// The comparer stays a plain runtime IComparer<TKey> - same §9.2 reasoning
// BinarySearch/MergeSort already establish: any TKey, any total order, is a
// valid comparer, an open-ended space no witness could usefully close over.
internal sealed class IntervalSet<TKey>
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

    private int LowerBoundByEnd(TKey start)
        => BinarySearch.LowerBound<TKey, IntervalEndsView>(new IntervalEndsView(_intervals), start, _comparer);

    private readonly struct IntervalEndsView(DynamicArray<(TKey Start, TKey End)> intervals) : IRandomAccessSequence<TKey>
    {
        public int Length => intervals.Count;

        public TKey Get(int index) => intervals.Get(index).End;
    }
}
