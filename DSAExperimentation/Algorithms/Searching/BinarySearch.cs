using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Searching;

// Requires a finite IRandomAccessSequence<Element> that is already sorted per the supplied
// comparer (i < j => comparer.Compare(sequence.Get(i), sequence.Get(j)) <= 0) - an
// unchecked precondition this algorithm leans on but never verifies, the same way
// ShortestPath leans on non-negative edge weights without checking them. Also
// depends on Get being O(1) (see IRandomAccessSequence): a representation with O(n)
// Get would silently degrade this from O(log n) to O(n log n), with no compiler
// error and no failing test to catch it. Duplicate targets resolve to some matching
// index, not necessarily the leftmost one - a real subtlety of plain binary search.
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
