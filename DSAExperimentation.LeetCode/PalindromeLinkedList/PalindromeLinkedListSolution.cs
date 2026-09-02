using DSAExperimentation.DataStructures.SinglyLinkedList;
using IntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.PalindromeLinkedList;

// LeetCode 234. Palindrome Linked List: decide whether a singly linked list reads
// the same forwards and backwards.
//
// Only one strategy is migrated here - the one real implementation that existed
// before this pass, previously a private helper duplicated into the test. The
// benchmark class was a compile-smoke placeholder (Baseline() => 1,
// PrimitiveComposed() => 1) that took no input and computed nothing, so there was
// no second arm to reconcile this against.
internal static class PalindromeLinkedListSolution
{
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
