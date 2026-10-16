using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.SortList;

// LeetCode 148. Sort List: sort a singly linked list in ascending order.
//
// The original test's private helper and the original benchmark's two [Benchmark]
// arms (both an unimplemented compile-smoke placeholder returning the literal 1)
// agreed on nothing real. Both strategies here flatten the list's values into an
// array, sort it, then rebuild a fresh list from the sorted values, and differ only
// in the sort: the BCL's Array.Sort, or this repo's own MergeSort.
internal static class SortListSolution
{
    // The textbook answer: the BCL's own sort, the arm this repo's MergeSort is measured
    // against (ARCHITECTURE 17.5).
    public static SinglyLinkedListNode<int>? SortByArraySort(SinglyLinkedListNode<int>? head)
    {
        var array = LeetCodeWireFormat.FromLinkedList(head);
        Array.Sort(array);

        return LeetCodeWireFormat.ToLinkedList(array);
    }

    public static SinglyLinkedListNode<int>? SortByMergeSortOverSequence(SinglyLinkedListNode<int>? head)
    {
        var array = LeetCodeWireFormat.FromLinkedList(head);
        MergeSort.Sort(array);

        return LeetCodeWireFormat.ToLinkedList(array);
    }
}
