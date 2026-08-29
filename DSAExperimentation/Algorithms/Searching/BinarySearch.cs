using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Searching;

// Requires a finite IRandomAccessSequence<Element> that is already sorted per the supplied
// comparer (i < j => comparer.Compare(sequence.Get(i), sequence.Get(j)) <= 0) - an
// unchecked precondition this algorithm leans on but never verifies, the same way
// ShortestPath leans on non-negative edge weights without checking them. Also
// depends on Get being O(1) (see IRandomAccessSequence): a representation with O(n)
// Get would silently degrade this from O(log n) to O(n log n), with no compiler
// error and no failing test to catch it. Find's duplicate targets resolve to some
// matching index, not necessarily the leftmost one - use LowerBound/UpperBound below
// when the leftmost/rightmost occurrence (or the whole run of equal elements) matters.
internal static class BinarySearch
{
    public static int? Find<Element, TSequence>(TSequence sequence, Element target)
        where TSequence : struct, IRandomAccessSequence<Element>
        where Element : IComparable<Element>
        => Find<Element, TSequence>(sequence, target, Comparer<Element>.Default);

    public static int? Find<Element, TSequence>(TSequence sequence, Element target, IComparer<Element> comparer)
        where TSequence : struct, IRandomAccessSequence<Element>
    {
        var range = new SearchRange(0, sequence.Length - 1);

        while (range.Low <= range.High)
        {
            var step = ProbeMidpoint(sequence, target, comparer, range);

            if (step.Found is not null)
            {
                return step.Found;
            }

            range = step.NextRange;
        }

        return null;
    }

    // The leftmost index at which target could be inserted without disturbing sort
    // order - the first index whose element is not less than target. Unlike Find, this
    // never returns null: an all-smaller sequence yields sequence.Length (append at the
    // end), and a run of duplicates always resolves to its first occurrence.
    public static int LowerBound<Element, TSequence>(TSequence sequence, Element target)
        where TSequence : struct, IRandomAccessSequence<Element>
        where Element : IComparable<Element>
        => LowerBound<Element, TSequence>(sequence, target, Comparer<Element>.Default);

    public static int LowerBound<Element, TSequence>(TSequence sequence, Element target, IComparer<Element> comparer)
        where TSequence : struct, IRandomAccessSequence<Element>
        => BoundSearch<Element, TSequence>(sequence, target, comparer, BoundKind.Lower);

    // The leftmost index whose element is strictly greater than target - the insertion
    // point that lands after every existing occurrence of target. [LowerBound,
    // UpperBound) is exactly the run of indices equal to target, empty when target is
    // absent.
    public static int UpperBound<Element, TSequence>(TSequence sequence, Element target)
        where TSequence : struct, IRandomAccessSequence<Element>
        where Element : IComparable<Element>
        => UpperBound<Element, TSequence>(sequence, target, Comparer<Element>.Default);

    public static int UpperBound<Element, TSequence>(TSequence sequence, Element target, IComparer<Element> comparer)
        where TSequence : struct, IRandomAccessSequence<Element>
        => BoundSearch<Element, TSequence>(sequence, target, comparer, BoundKind.Upper);

    // Lower and Upper differ only in whether an element equal to target counts as
    // "before" it (Lower: no, so the run's first occurrence is kept; Upper: yes, so
    // the search lands just past the run's last occurrence) - the one comparison the
    // two previously-separate loops disagreed on.
    private static int BoundSearch<Element, TSequence>(
        TSequence sequence, Element target, IComparer<Element> comparer, BoundKind kind)
        where TSequence : struct, IRandomAccessSequence<Element>
    {
        var low = 0;
        var high = sequence.Length;

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);
            var comparison = comparer.Compare(sequence.Get(mid), target);
            var isBeforeTarget = kind == BoundKind.Lower ? comparison < 0 : comparison <= 0;

            if (isBeforeTarget)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        return low;
    }

    private enum BoundKind
    {
        Lower,
        Upper,
    }

    // Evaluates one bisection step: compares the midpoint to target, and either
    // returns it (found) or narrows range toward whichever half target must be in if
    // the sortedness precondition holds.
    private static BisectionStep ProbeMidpoint<Element, TSequence>(
        TSequence sequence, Element target, IComparer<Element> comparer, SearchRange range)
        where TSequence : struct, IRandomAccessSequence<Element>
    {
        // low + (high-low)/2, not (low+high)/2, so this never overflows.
        var mid = range.Low + ((range.High - range.Low) / AlgorithmConstants.HalvingFactor);
        var comparison = comparer.Compare(sequence.Get(mid), target);

        if (comparison == 0)
        {
            return new BisectionStep(mid, range);
        }

        return comparison < 0
            ? new BisectionStep(null, range with { Low = mid + 1 })
            : new BisectionStep(null, range with { High = mid - 1 });
    }

    private readonly record struct BisectionStep(int? Found, SearchRange NextRange);
}
