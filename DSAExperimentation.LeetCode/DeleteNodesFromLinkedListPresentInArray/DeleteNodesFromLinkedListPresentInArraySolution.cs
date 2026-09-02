using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.DeleteNodesFromLinkedListPresentInArray;

// LeetCode 3217. Delete Nodes From Linked List Present in Array: splice out
// every node whose value appears in nums, keeping everything else in order.
//
// Both strategies are the same dummy-head single pass; they differ only in how
// membership in nums is answered, exactly the TwoSum shape (nested-scan
// baseline vs. one repo container carrying the O(1) lookup).
internal static class DeleteNodesFromLinkedListPresentInArraySolution
{
    // The textbook answer: no auxiliary structure at all, just a linear scan of
    // nums per node - O(n * m) where m = nums.Length. Deliberately written
    // without this repo's primitives, the arm the Set strategy has to beat.
    public static SinglyLinkedListNode<int>? ModifiedListByArrayScan(int[] nums, SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var tail = dummy;

        for (var node = head; node is not null; node = node.Next)
        {
            if (Array.IndexOf(nums, node.Value) < 0)
            {
                tail.Next = node;
                tail = node;
            }
        }

        tail.Next = null;
        return dummy.Next;
    }

    // One O(n + m) pass: nums goes into this repo's own Set<int> once, then
    // every node's membership check is a single Has lookup instead of a rescan.
    public static SinglyLinkedListNode<int>? ModifiedListBySetFilter(int[] nums, SinglyLinkedListNode<int>? head)
        => ModifiedListBySetFilter(new Set<int>(nums), head);

    public static SinglyLinkedListNode<int>? ModifiedListBySetFilter(Set<int> nums, SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var tail = dummy;

        for (var node = head; node is not null; node = node.Next)
        {
            if (!nums.Has(node.Value))
            {
                tail.Next = node;
                tail = node;
            }
        }

        tail.Next = null;
        return dummy.Next;
    }
}
