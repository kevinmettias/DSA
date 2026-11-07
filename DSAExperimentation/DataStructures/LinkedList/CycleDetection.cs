namespace DSAExperimentation.DataStructures.LinkedList;

// Floyd's walk follows only forward links, so it asks for the forward-linked node,
// LinkedNode<TNode, TValue>, and nothing more: it runs over every configuration of the one linked
// node, singly, doubly or with a random pointer, and a back link is not a member it could reach.
// It sits beside the node template it walks (§13.5), not in Algorithms/, the way InOrderTraversal
// sits beside the binary tree. Next is the template's own field, so each step is one load in code
// shared over the node type too (§13.11).
/// <summary>
/// Detects whether a linked list has a cycle, and where it begins, in constant extra space.
/// </summary>
public static class CycleDetection
{
    // Null/acyclic-safe: returns false with no hang if there is no cycle.
    /// <summary>Tests whether a linked list has a cycle.</summary>
    /// <typeparam name="TNode">The list's node configuration.</typeparam>
    /// <typeparam name="TValue">The value each node holds.</typeparam>
    /// <param name="head">The list's first node, or null for an empty list.</param>
    /// <returns>True when the list has a cycle; false when it ends, or is empty.</returns>
    public static bool HasCycle<TNode, TValue>(LinkedNode<TNode, TValue>? head)
        where TNode : LinkedNode<TNode, TValue>
        => FindMeetingPoint(head) is not null;

    // Floyd's tortoise-and-hare to find a meeting point inside the cycle, then the standard second
    // phase - reset one pointer to head, advance both one step at a time - to find the cycle's
    // entry node. Returns null if there is no cycle.
    /// <summary>Finds the node at which a linked list enters its cycle.</summary>
    /// <typeparam name="TNode">The list's node configuration.</typeparam>
    /// <typeparam name="TValue">The value each node holds.</typeparam>
    /// <param name="head">The list's first node, or null for an empty list.</param>
    /// <returns>The node the list enters its cycle at, or null when it has no cycle.</returns>
    public static TNode? FindCycleStart<TNode, TValue>(LinkedNode<TNode, TValue>? head)
        where TNode : LinkedNode<TNode, TValue>
    {
        var meetingPoint = FindMeetingPoint(head);

        // presumption: allow -- meetingPoint is only non-null when FindMeetingPoint's walk actually
        // advanced at least once, which is only possible starting from a non-null head.
        return meetingPoint is null ? null : FindEntryNode(head!, meetingPoint);
    }

    // The two walks meet on the entry node, and the one from the meeting point is the one typed as
    // the configuration, so that is the one returned: the head arrives typed as the node template.
    private static TNode FindEntryNode<TNode, TValue>(
        LinkedNode<TNode, TValue> head, TNode meetingPoint)
        where TNode : LinkedNode<TNode, TValue>
    {
        var fromHead = head;
        var fromMeetingPoint = meetingPoint;

        while (!ReferenceEquals(fromHead, fromMeetingPoint))
        {
            // presumption: allow -- every node walked from head must eventually reach the cycle
            // FindMeetingPoint already proved exists (a node has exactly one forward link, so there
            // is no branch off that path), and once inside the cycle .Next never terminates - so
            // this walk can never hit a null .Next before ReferenceEquals closes the loop above.
            fromHead = fromHead.Next!;
            fromMeetingPoint = fromMeetingPoint.Next!;
        }

        return fromMeetingPoint;
    }

    // The meeting point is a node the slow walk stepped onto, so it comes back typed as the
    // configuration without a cast.
    private static TNode? FindMeetingPoint<TNode, TValue>(LinkedNode<TNode, TValue>? head)
        where TNode : LinkedNode<TNode, TValue>
    {
        var slow = head;
        var fast = head;

        while (fast?.Next is not null)
        {
            // presumption: allow -- fast always leads slow by construction (fast advances two steps
            // per iteration, slow one), so the loop condition already proving fast/fast.Next
            // non-null proves slow reached this same position, and therefore is non-null, on an
            // earlier or equal iteration.
            var stepped = slow!.Next;
            slow = stepped;
            fast = fast.Next.Next;

            if (ReferenceEquals(stepped, fast))
            {
                return stepped;
            }
        }

        return null;
    }
}
