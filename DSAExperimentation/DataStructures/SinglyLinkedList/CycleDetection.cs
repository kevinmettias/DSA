namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// Hardwired directly against SinglyLinkedListNode<T>.Next, the same "no capability
// interface, no second representation to be generic over" shape InOrderTraversal
// has for BinaryTreeNode - a linked list node has exactly one successor, so there
// is nothing here for a Representation contract to abstract over, and per
// ARCHITECTURE.md §5 step 7 / §13.5 that means this co-locates under
// DataStructures/ rather than Algorithms/.
/// <summary>
/// Detects whether a singly linked list has a cycle, and where it begins, in constant extra space.
/// </summary>
public static class CycleDetection
{
    // Null/acyclic-safe: returns false with no hang if there is no cycle.
    /// <summary>Tests whether a singly linked list has a cycle.</summary>
    /// <param name="head">The list's first node, or null for an empty list.</param>
    /// <returns>True when the list has a cycle; false when it ends, including when it is empty.</returns>
    public static bool HasCycle<TValue>(SinglyLinkedListNode<TValue>? head)
        => FindMeetingPoint(head) is not null;

    // Floyd's tortoise-and-hare to find a meeting point inside the cycle, then the
    // standard second phase - reset one pointer to head, advance both one step at
    // a time - to find the cycle's entry node. Returns null if there is no cycle.
    /// <summary>Finds the node at which a singly linked list enters its cycle.</summary>
    /// <param name="head">The list's first node, or null for an empty list.</param>
    /// <returns>The node the list enters its cycle at, or null when the list has no cycle.</returns>
    public static SinglyLinkedListNode<TValue>? FindCycleStart<TValue>(SinglyLinkedListNode<TValue>? head)
    {
        var meetingPoint = FindMeetingPoint(head);

        // presumption: allow -- meetingPoint is only non-null when FindMeetingPoint's
        // walk actually advanced at least once, which is only possible starting from
        // a non-null head.
        return meetingPoint is null ? null : FindEntryNode(head!, meetingPoint);
    }

    private static SinglyLinkedListNode<TValue> FindEntryNode<TValue>(
        SinglyLinkedListNode<TValue> head, SinglyLinkedListNode<TValue> meetingPoint)
    {
        var fromHead = head;
        var fromMeetingPoint = meetingPoint;

        while (!ReferenceEquals(fromHead, fromMeetingPoint))
        {
            // presumption: allow -- every node walked from head must eventually reach
            // the cycle FindMeetingPoint already proved exists (a singly linked node
            // has exactly one successor, so there is no branch off that path), and
            // once inside the cycle .Next never terminates - so this walk can never
            // hit a null .Next before ReferenceEquals closes the loop above.
            fromHead = fromHead.Next!;
            fromMeetingPoint = fromMeetingPoint.Next!;
        }

        return fromHead;
    }

    private static SinglyLinkedListNode<TValue>? FindMeetingPoint<TValue>(SinglyLinkedListNode<TValue>? head)
    {
        var slow = head;
        var fast = head;

        while (fast?.Next is not null)
        {
            // presumption: allow -- fast always leads slow by construction (fast
            // advances two steps per iteration, slow one), so the loop condition
            // already proving fast/fast.Next non-null proves slow reached this same
            // position, and therefore is non-null, on an earlier or equal iteration.
            slow = slow!.Next;
            fast = fast.Next.Next;

            if (ReferenceEquals(slow, fast))
            {
                return slow;
            }
        }

        return null;
    }
}
