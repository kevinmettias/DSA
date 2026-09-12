using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.QueueReconstructionByHeight;

// LeetCode 406. Queue Reconstruction by Height: sort people tallest-first
// (ties broken by k ascending), then insert each person at index k. Every
// already-placed person is at least as tall, so k always lands exactly where
// the LeetCode rule requires, and every later, shorter insertion leaves it
// undisturbed.
//
// This problem's insight - why descending-height order makes the insert
// index always valid - doesn't leave room for a differently-shaped naive
// alternative the way HIndex/RussianDollEnvelopes do, so both strategies run
// the same sort-then-insert algorithm and differ only in what each step is
// built from. ReconstructQueueByArraySortListInsert uses BCL Array.Sort and
// List<T>.Insert; ReconstructQueueByMergeSortDynamicArrayInsert instead
// composes this repo's own MergeSort.Sort<Element,TSequence> over an
// ArrayIndexedSequence - the same custom-comparer shape
// RussianDollEnvelopesSolution already exercises - and DynamicArray<Element>.
internal static class QueueReconstructionByHeightSolution
{
    public static int[][] ReconstructQueueByArraySortListInsert(int[][] people) =>
        ReconstructQueueByArraySortListInsert(BuildItems(people));

    // The textbook arm: BCL Array.Sort plus List<T>.Insert at the shifted index.
    // Deliberately written without this repo's primitives - it is the arm the
    // MergeSort/DynamicArray strategy below has to justify itself against.
    public static int[][] ReconstructQueueByArraySortListInsert((int Height, int K)[] people)
    {
        var items = ((int Height, int K)[])people.Clone();
        Array.Sort(items, (a, b) => a.Height != b.Height ? b.Height.CompareTo(a.Height) : a.K.CompareTo(b.K));

        var queue = new List<(int Height, int K)>();

        foreach (var person in items)
        {
            queue.Insert(person.K, person);
        }

        return ToResultArray(queue);
    }

    public static int[][] ReconstructQueueByMergeSortDynamicArrayInsert(int[][] people) =>
        ReconstructQueueByMergeSortDynamicArrayInsert(BuildItems(people));

    // This repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence for the
    // O(n log n) sort, then DynamicArray<Element>.Insert at the shifted index.
    public static int[][] ReconstructQueueByMergeSortDynamicArrayInsert((int Height, int K)[] people)
    {
        var items = ((int Height, int K)[])people.Clone();
        var byHeightDescendingThenKAscending = Comparer<(int Height, int K)>.Create(
            (a, b) => a.Height != b.Height ? b.Height.CompareTo(a.Height) : a.K.CompareTo(b.K));

        MergeSort.Sort<(int Height, int K), ArrayIndexedSequence<(int Height, int K)>>(
            new ArrayIndexedSequence<(int Height, int K)>(items), byHeightDescendingThenKAscending);

        var queue = new DynamicArray<(int Height, int K)>();

        foreach (var person in items)
        {
            queue.Insert(person.K, person);
        }

        return ToResultArray(queue);
    }

    private static (int Height, int K)[] BuildItems(int[][] people) =>
        people.Select(p => (Height: p[0], K: p[1])).ToArray();

    private static int[][] ToResultArray(List<(int Height, int K)> queue)
    {
        var result = new int[queue.Count][];

        for (var i = 0; i < queue.Count; i++)
        {
            var (height, k) = queue[i];
            result[i] = [height, k];
        }

        return result;
    }

    private static int[][] ToResultArray(DynamicArray<(int Height, int K)> queue)
    {
        var result = new int[queue.Count][];

        for (var i = 0; i < queue.Count; i++)
        {
            var (height, k) = queue.Get(i);
            result[i] = [height, k];
        }

        return result;
    }
}
