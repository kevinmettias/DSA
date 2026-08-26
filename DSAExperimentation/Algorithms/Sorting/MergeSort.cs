using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Sorting;

// Sort is O(n log n) - a claim that depends on TSequence's Get and Set both being O(1) (see
// IIndexedSequence.cs) and on the scratch buffer being allocated exactly once per Sort call and
// reused across every merge step: reallocating per merge call would stay correct but silently add
// allocation/GC pressure that erodes the claim in practice, the same "no compiler error, no
// failing test" failure mode ARCHITECTURE.md §8 describes for a slower Representation, just
// self-inflicted by this type rather than inherited from one.
//
// No precondition law analogous to BinarySearch's sortedness: sortedness is this algorithm's
// postcondition, not an assumed input. The comparer stays a plain IComparer<Element> parameter for the
// same reason BinarySearch's does (ARCHITECTURE.md §9.2) - any Element, any total order, is a valid
// comparer, an open-ended space no witness could usefully close over.
internal static class MergeSort
{
    public static void Sort<Element, TSequence>(TSequence sequence)
        where TSequence : struct, IIndexedSequence<Element>
        where Element : IComparable<Element>
        => Sort<Element, TSequence>(sequence, Comparer<Element>.Default);

    public static void Sort<Element, TSequence>(TSequence sequence, IComparer<Element> comparer)
        where TSequence : struct, IIndexedSequence<Element>
    {
        if (sequence.Length <= 1)
        {
            return;
        }

        var buffer = new Element[sequence.Length];
        SortRange(sequence, comparer, buffer, new SortBounds(0, sequence.Length - 1));
    }

    private static void SortRange<Element, TSequence>(TSequence sequence, IComparer<Element> comparer, Element[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<Element>
    {
        if (bounds.Low >= bounds.High)
        {
            return;
        }

        var mid = Midpoint(bounds);

        SortRange(sequence, comparer, buffer, new SortBounds(bounds.Low, mid));
        SortRange(sequence, comparer, buffer, new SortBounds(mid + 1, bounds.High));
        Merge(sequence, comparer, buffer, bounds);
    }

    private static int Midpoint(SortBounds bounds) => bounds.Low + ((bounds.High - bounds.Low) / AlgorithmConstants.HalvingFactor);

    // Merges the two already-sorted halves [Low, mid] and [mid+1, High] into buffer, then copies
    // the merged result back into sequence. mid is recomputed rather than threaded through as a
    // parameter: it's a pure function of bounds, always identical to the split point SortRange
    // used to produce these same two halves.
    private static void Merge<Element, TSequence>(TSequence sequence, IComparer<Element> comparer, Element[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<Element>
    {
        MergeRunsIntoBuffer(sequence, comparer, buffer, bounds);
        CopyBufferIntoSequence(sequence, buffer, bounds);
    }

    // A single counted loop rather than the usual textbook "merge-while-both-remain, then drain
    // whichever run is left" three loops: right > bounds.High short-circuits the comparison once
    // the right run is exhausted, so the left run drains through the same branch that compares
    // during normal merging, and left <= mid guards the symmetric case once the left run is
    // exhausted. Exactly bounds.High - bounds.Low + 1 elements exist across both runs, so the two
    // cursors always finish exactly as next reaches bounds.High - neither can be read past its run.
    private static void MergeRunsIntoBuffer<Element, TSequence>(TSequence sequence, IComparer<Element> comparer, Element[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<Element>
    {
        var mid = Midpoint(bounds);
        var left = bounds.Low;
        var right = mid + 1;

        for (var next = bounds.Low; next <= bounds.High; next++)
        {
            var takeLeft = left <= mid && (right > bounds.High || comparer.Compare(sequence.Get(left), sequence.Get(right)) <= 0);
            buffer[next] = takeLeft ? sequence.Get(left++) : sequence.Get(right++);
        }
    }

    private static void CopyBufferIntoSequence<Element, TSequence>(TSequence sequence, Element[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<Element>
    {
        for (var i = bounds.Low; i <= bounds.High; i++)
        {
            sequence.Set(i, buffer[i]);
        }
    }
}
