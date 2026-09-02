using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 160 - the two-pointer walk's cost depends
// only on each list's length, not on node values, so this builds two chains of
// a given length that converge onto one shared tail node.
internal static class IntersectionOfTwoLinkedListsWorkloads
{
    public static (SinglyLinkedListNode<int> HeadA, SinglyLinkedListNode<int> HeadB) Build(int prefixLength)
    {
        var shared = new SinglyLinkedListNode<int>(-1);
        var headA = Prepend(prefixLength, shared);
        var headB = Prepend(prefixLength * 2, shared);

        return (headA, headB);
    }

    private static SinglyLinkedListNode<int> Prepend(int count, SinglyLinkedListNode<int> tail)
    {
        var head = tail;

        for (var i = 0; i < count; i++)
        {
            head = new SinglyLinkedListNode<int>(i) { Next = head };
        }

        return head;
    }
}
