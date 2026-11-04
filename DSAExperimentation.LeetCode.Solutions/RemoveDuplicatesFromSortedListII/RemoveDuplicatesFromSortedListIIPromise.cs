using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.RemoveDuplicatesFromSortedListII;

// LC 82's promise about its input: the list is sorted in ascending order. LeetCode's judge never
// poses an input that breaks it. Out of order, equal values are not neighbours, and the two
// strategies find different ones. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class RemoveDuplicatesFromSortedListIIPromise
{
    public static bool IsKeptBy(SinglyLinkedListNode<int>? head)
    {
        for (var node = head; node?.Next is not null; node = node.Next)
        {
            if (node.Value > node.Next.Value)
            {
                return false;
            }
        }

        return true;
    }
}
