using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Searching;

// The sorted distinct values of a fixed input, so a FenwickTree or SegmentTree can be sized by Count and
// indexed by rank instead of by value - coordinate compression, the step every such solution used to
// spell out as Distinct, OrderBy, ToArray and a LowerBound per lookup. Ranks are 0-based, like the
// trees' own indices.
//
// By §5 step 7 this would be a data structure, hardwired to its one array. It builds with MergeSort and
// answers with BinarySearch, though, so it lives in Algorithms/ for the reason HammingDistances does
// (§17.6): a DataStructures/ type composing either would invert the tiers. It sits in Searching because
// after construction all it does is bisect; the sort runs once.
//
// RankOf is for values the set was built from and throws for any other value, so a caller that passes a
// derived value - twice an element, a prefix total minus a bound - expecting an exact rank finds out at
// once rather than reading a neighbour's slot. LowerBound and UpperBound are BinarySearch's insertion
// points, for thresholds that need not be members: the count of coordinates below, or at or below, a
// value. Duplicates are told apart by CompareTo, not Equals, so the set agrees with the order the
// searches use. O(n log n) to build and O(log n) per query, given the view's O(1) Get (§8).
internal sealed class CompressedCoordinates<Element>
    where Element : IComparable<Element>
{
    private const string NotACoordinateMessage = "The value is not one of the coordinates this set was built from.";

    // Sorted ascending; the first Count entries are the distinct coordinates, and the rest is what the
    // in-place deduplication left behind, never read.
    private readonly Element[] _sorted;

    private OffsetSequence<Element> Distinct => new(_sorted, 0, Count);

    public int Count { get; }

    public CompressedCoordinates(ReadOnlySpan<Element> values)
    {
        _sorted = values.ToArray();
        MergeSort.Sort(_sorted);
        Count = KeepDistinct(_sorted);
    }

    public int RankOf(Element value)
    {
        var rank = LowerBound(value);
        var isCoordinate = rank < Count && _sorted[rank].CompareTo(value) == 0;

        if (!isCoordinate)
        {
            throw new ArgumentException(NotACoordinateMessage, nameof(value));
        }

        return rank;
    }

    public int LowerBound(Element value) => BinarySearch.LowerBound<Element, OffsetSequence<Element>>(Distinct, value);

    public int UpperBound(Element value) => BinarySearch.UpperBound<Element, OffsetSequence<Element>>(Distinct, value);

    // Moves each value that differs from the last one kept down to the next free slot, and answers how
    // many were kept. Sorted input makes every duplicate adjacent to the copy already kept.
    private static int KeepDistinct(Element[] sorted)
    {
        if (sorted.Length == 0)
        {
            return 0;
        }

        var kept = 1;

        for (var next = 1; next < sorted.Length; next++)
        {
            if (sorted[next].CompareTo(sorted[kept - 1]) != 0)
            {
                sorted[kept++] = sorted[next];
            }
        }

        return kept;
    }
}
