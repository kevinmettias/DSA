using DSAExperimentation.DataStructures.Sorting;

namespace DSAExperimentation.Algorithms.Sorting;

// Sort is O(n log n) - a claim that depends on TSequence's Get and Set both being O(1) (see
// IIndexedSequence.cs) and on the scratch buffer being allocated exactly once per Sort call and
// reused across every merge step: reallocating per merge call would stay correct but silently add
// allocation/GC pressure that erodes the claim in practice, the same "no compiler error, no
// failing test" failure mode ARCHITECTURE.md §8 describes for a slower Representation, just
// self-inflicted by this type rather than inherited from one.
//
// No precondition law analogous to BinarySearch's sortedness: sortedness is this algorithm's
// postcondition, not an assumed input. The comparer stays a plain IComparer<T> parameter for the
// same reason BinarySearch's does (ARCHITECTURE.md §9.2) - any T, any total order, is a valid
// comparer, an open-ended space no witness could usefully close over.
internal static class MergeSort
{
    private const int HalvingFactor = 2;

    public static void Sort<T, TSequence>(TSequence sequence)
        where TSequence : struct, IIndexedSequence<T>
        where T : IComparable<T>
        => Sort<T, TSequence>(sequence, Comparer<T>.Default);

    public static void Sort<T, TSequence>(TSequence sequence, IComparer<T> comparer)
        where TSequence : struct, IIndexedSequence<T>
    {
        if (sequence.Length <= 1)
        {
            return;
        }

        var buffer = new T[sequence.Length];
        SortRange(sequence, comparer, buffer, new SortBounds(0, sequence.Length - 1));
    }

    private static void SortRange<T, TSequence>(TSequence sequence, IComparer<T> comparer, T[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<T>
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

    private static int Midpoint(SortBounds bounds) => bounds.Low + ((bounds.High - bounds.Low) / HalvingFactor);

    // Merges the two already-sorted halves [Low, mid] and [mid+1, High] into buffer, then copies
    // the merged result back into sequence. mid is recomputed rather than threaded through as a
    // parameter: it's a pure function of bounds, always identical to the split point SortRange
    // used to produce these same two halves.
    private static void Merge<T, TSequence>(TSequence sequence, IComparer<T> comparer, T[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<T>
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
    private static void MergeRunsIntoBuffer<T, TSequence>(TSequence sequence, IComparer<T> comparer, T[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<T>
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

    private static void CopyBufferIntoSequence<T, TSequence>(TSequence sequence, T[] buffer, SortBounds bounds)
        where TSequence : struct, IIndexedSequence<T>
    {
        for (var i = bounds.Low; i <= bounds.High; i++)
        {
            sequence.Set(i, buffer[i]);
        }
    }
}
