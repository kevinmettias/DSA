using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.InsertionSortList;

// LeetCode 147. Insertion Sort List: sort a singly linked list using the
// insertion sort algorithm.
//
// There is only one strategy here: the original test's private helper and
// the original benchmark's two [Benchmark] arms (both an unimplemented
// compile-smoke placeholder returning the literal 1) agreed on nothing real,
// so this is the actual algorithm - a dummy-headed sorted prefix that each
// incoming node walks forward into and splices behind, exactly what LC 147
// asks for by name.
internal static class InsertionSortListSolution
{
    public static SinglyLinkedListNode<int>? SortByDummyHeadInsertion(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);

        for (var node = head; node is not null;)
        {
            var next = node.Next;
            var insertAfter = dummy;

            while (insertAfter.Next is not null && insertAfter.Next.Value < node.Value)
            {
                insertAfter = insertAfter.Next;
            }

            node.Next = insertAfter.Next;
            insertAfter.Next = node;
            node = next;
        }

        return dummy.Next;
    }
}
