using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.DeleteNodeInALinkedList;

// LeetCode 237. Delete Node in a Linked List: given a node (guaranteed not to be
// the tail) with no access to the list's head, delete it in-place.
//
// LeetCode's own signature never hands over the head, so the node passed in can
// never literally be unlinked from its predecessor - the only move available is to
// copy the next node's value onto this one and then splice the next node out,
// which makes the given node indistinguishable from having been removed. Only one
// strategy exists in this repo's coverage today - the test's private Delete
// helper, previously duplicated; the benchmark was a compile-smoke placeholder
// with no second arm to reconcile.
internal static class DeleteNodeInALinkedListSolution
{
    public static void DeleteByNextValueCopy(SinglyLinkedListNode<int> node)
    {
        // LC 237 guarantees node is never the list's tail, so Next is always present.
        node.Value = node.Next!.Value;
        node.Next = node.Next.Next;
    }
}
