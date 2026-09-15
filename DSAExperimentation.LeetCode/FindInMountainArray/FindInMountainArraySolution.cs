using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindInMountainArray;

// LeetCode 1095. Find in Mountain Array: return the smallest index at which a
// strictly-up-then-strictly-down array holds target, or -1.
//
// FindIndexByPeakBisection locates the peak with a hand-rolled SearchRange
// bisection - the one part of this problem with no existing primitive to compose,
// since the peak is found by comparing neighbours rather than against a target -
// and then hands each monotonic slope to BinarySearch.Find as an ordinary sorted
// range: the ascending slope with the default comparer, the descending slope with
// a reversed one, leaning on the "any total order is a valid comparer" contract
// BinarySearch.Find's own doc comment states (ARCHITECTURE.md Sec 9.2). Neither
// slope is copied - DataStructures.Sequence's OffsetSequence<int> is already a
// window into a sub-range of the backing array. The ascending slope is searched
// first, so a value present on both slopes reports its smaller index.
internal static class FindInMountainArraySolution
{
    private static readonly IComparer<int> Descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

    // The textbook O(n) scan every one-call-at-a-time approach degenerates to.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy has to beat - and it returns the first match, which is the
    // smallest index by construction.
    public static int FindIndexByLinearScan(int[] mountain, int target)
    {
        for (var i = 0; i < mountain.Length; i++)
        {
            if (mountain[i] == target)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Bisect to the peak, then two BinarySearch.Find passes - one per slope.
    public static int FindIndexByPeakBisection(int[] mountain, int target)
    {
        var peakIndex = FindPeakIndex(mountain);

        var ascending = new OffsetSequence<int>(mountain, start: 0, peakIndex + 1);
        var ascendingIndex = BinarySearch.Find(ascending, target);

        if (ascendingIndex is not null)
        {
            return ascendingIndex.Value;
        }

        var descending = new OffsetSequence<int>(mountain, peakIndex, mountain.Length - peakIndex);
        var descendingIndex = BinarySearch.Find(descending, target, Descending);

        return descendingIndex is null
            ? LeetCodeAnswer.None
            : AbsoluteIndex(descendingIndex.Value, peakIndex);
    }

    // "Is this slot still climbing?" is monotone across the array - true on the
    // whole ascending slope and false from the peak onward - so bisecting on it
    // converges on the peak index.
    private static int FindPeakIndex(int[] mountain)
    {
        var sequence = new OffsetSequence<int>(mountain, start: 0, mountain.Length);
        var range = new SearchRange(0, mountain.Length - 1);

        while (range.Low < range.High)
        {
            var mid = range.Low + ((range.High - range.Low) / AlgorithmConstants.HalvingFactor);

            range = IsStillClimbing(sequence, mid)
                ? NarrowToUpperHalf(range, mid)
                : NarrowToLowerHalf(range, mid);
        }

        return range.Low;
    }

    // The predicate the peak bisection converges on, read off the slot and its
    // neighbour: still rising puts the peak at mid or beyond it.
    private static bool IsStillClimbing(OffsetSequence<int> sequence, int mid)
        => sequence.Get(mid) < sequence.Get(mid + 1);

    // Which half survives the comparison - the upper one starts past mid, the lower
    // one keeps mid, since mid is only known to be below the peak.
    private static SearchRange NarrowToUpperHalf(SearchRange range, int mid)
        => range with { Low = mid + 1 };

    private static SearchRange NarrowToLowerHalf(SearchRange range, int mid)
        => range with { High = mid };

    // The window's local index is not the array's index; the window's start has to
    // come back on.
    private static int AbsoluteIndex(int localIndex, int windowStart) => localIndex + windowStart;
}
