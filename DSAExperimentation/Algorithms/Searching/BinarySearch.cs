using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Searching;

// Requires a finite IRandomAccessSequence<T> that is already sorted per the supplied
// comparer (i < j => comparer.Compare(sequence.Get(i), sequence.Get(j)) <= 0) - an
// unchecked precondition this algorithm leans on but never verifies, the same way
// ShortestPath leans on non-negative edge weights without checking them. Also
// depends on Get being O(1) (see IRandomAccessSequence): a representation with O(n)
// Get would silently degrade this from O(log n) to O(n log n), with no compiler
// error and no failing test to catch it. Duplicate targets resolve to some matching
// index, not necessarily the leftmost one - a real subtlety of plain binary search.
internal static class BinarySearch
{
    private const int HalvingFactor = 2;

    public static int? Find<T, TSequence>(TSequence sequence, T target)
        where TSequence : struct, IRandomAccessSequence<T>
        where T : IComparable<T>
        => Find<T, TSequence>(sequence, target, Comparer<T>.Default);

    public static int? Find<T, TSequence>(TSequence sequence, T target, IComparer<T> comparer)
        where TSequence : struct, IRandomAccessSequence<T>
    {
        var range = new SearchRange(0, sequence.Length - 1);

        while (range.Low <= range.High)
        {
            var found = ProbeMidpoint(sequence, target, comparer, ref range);

            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    // Evaluates one bisection step: compares the midpoint to target, and either
    // returns it (found) or narrows range toward whichever half target must be in if
    // the sortedness precondition holds.
    private static int? ProbeMidpoint<T, TSequence>(
        TSequence sequence, T target, IComparer<T> comparer, ref SearchRange range)
        where TSequence : struct, IRandomAccessSequence<T>
    {
        // low + (high-low)/2, not (low+high)/2, so this never overflows.
        var mid = range.Low + ((range.High - range.Low) / HalvingFactor);
        var comparison = comparer.Compare(sequence.Get(mid), target);

        if (comparison == 0)
        {
            return mid;
        }

        if (comparison < 0)
        {
            range.Low = mid + 1;
        }
        else
        {
            range.High = mid - 1;
        }

        return null;
    }
}
