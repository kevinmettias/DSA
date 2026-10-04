using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.SortList;

// LeetCode 148. Sort List: sort a singly linked list in ascending order.
//
// There is only one strategy here: the original test's private helper and
// the original benchmark's two [Benchmark] arms (both an unimplemented
// compile-smoke placeholder returning the literal 1) agreed on nothing real,
// so this is the actual algorithm - flatten the list's values into an array,
// hand it to this repo's own MergeSort over an ArrayIndexedSequence view,
// then rebuild a fresh list from the sorted values.
internal static class SortListSolution
{
    public static SinglyLinkedListNode<int>? SortByMergeSortOverSequence(SinglyLinkedListNode<int>? head)
    {
        var array = LeetCodeWireFormat.FromLinkedList(head);
        MergeSort.Sort(array);

        return LeetCodeWireFormat.ToLinkedList(array);
    }
}
