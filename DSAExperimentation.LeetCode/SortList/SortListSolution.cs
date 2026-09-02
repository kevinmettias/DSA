using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SinglyLinkedList;

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
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var array = values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(array));

        return BuildList(array);
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);

            if (tail is null)
            {
                head = node;
            }
            else
            {
                tail.Next = node;
            }

            tail = node;
        }

        return head;
    }
}
