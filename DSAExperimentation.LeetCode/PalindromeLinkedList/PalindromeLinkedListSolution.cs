using DSAExperimentation.DataStructures.SinglyLinkedList;
using IntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.PalindromeLinkedList;

// LeetCode 234. Palindrome Linked List: decide whether a singly linked list reads
// the same forwards and backwards.
//
// Two strategies. The first is the one real implementation that existed before this
// pass, previously a private helper duplicated into the test; the second is the
// fast/slow pointer walk above it, which reverses the second half in place instead of
// holding a copy. The old benchmark class was a compile-smoke placeholder
// (Baseline() => 1, PrimitiveComposed() => 1) that took no input and computed nothing,
// so there was no second arm to reconcile these against.
internal static class PalindromeLinkedListSolution
{
    // The in-place counterpart to the stack arm below: two pointers advance at
    // different speeds until the slow one reaches the midpoint, the second half is then
    // reversed by rewiring Next, and the two halves are compared node by node. It uses
    // O(1) extra space where the stack holds a full copy, at the cost of mutating the
    // list - so it reverses the second half back before returning, which restores the
    // caller's list and is what lets the benchmark reuse one cached chain across
    // iterations. For an odd length the middle node is simply skipped.
    public static bool IsPalindromeByFastSlowReversal(SinglyLinkedListNode<int>? head)
    {
        if (head is null || head.Next is null)
        {
            return true;
        }

        var slow = head;
        var fast = head;

        while (fast.Next is not null && fast.Next.Next is not null)
        {
            slow = slow.Next;
            fast = fast.Next.Next;
        }

        var reversedHalf = Reverse(slow.Next);
        var isPalindrome = HalvesMatch(head, reversedHalf);
        slow.Next = Reverse(reversedHalf);

        return isPalindrome;
    }

    // Walk the first half forward and the reversed second half from its new head; the
    // second half is never longer than the first, so exhausting it walks both.
    private static bool HalvesMatch(SinglyLinkedListNode<int> first, SinglyLinkedListNode<int>? secondHalf)
    {
        var left = first;
        var right = secondHalf;

        while (right is not null)
        {
            if (left.Value != right.Value)
            {
                return false;
            }

            left = left.Next!;
            right = right.Next;
        }

        return true;
    }

    // Rewires a segment's Next pointers so it reads back to front, returning the new
    // head; applied a second time to the same reversed segment it restores the original
    // order.
    private static SinglyLinkedListNode<int>? Reverse(SinglyLinkedListNode<int>? head)
    {
        SinglyLinkedListNode<int>? previous = null;
        var current = head;

        while (current is not null)
        {
            var next = current.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        return previous;
    }

    // Push every value walking forward, then walk forward a second time popping:
    // Stack<T>'s LIFO order hands values back most-recently-pushed-first - the
    // list's own reverse order - so comparing pop-by-pop against the forward walk
    // is exactly a forward/backward comparison without ever reversing the list.
    public static bool IsPalindromeByStackReversal(SinglyLinkedListNode<int>? head)
    {
        var reversed = new IntStack();

        for (var node = head; node is not null; node = node.Next)
        {
            reversed.Push(node.Value);
        }

        for (var node = head; node is not null; node = node.Next)
        {
            reversed.TryPop(out var value);

            if (value != node.Value)
            {
                return false;
            }
        }

        return true;
    }
}
