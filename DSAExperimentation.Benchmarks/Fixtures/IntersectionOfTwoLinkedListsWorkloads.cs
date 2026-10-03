using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 160 - the two-pointer walk's cost depends
// only on each list's length, not on node values, so this builds two chains of
// a given length that converge onto one shared tail node. Every value sits inside
// LC 160's [1, 10^5]: a prefix node holds its one-based position, and the shared
// tail holds the largest value, which no prefix position reaches.
internal static class IntersectionOfTwoLinkedListsWorkloads
{
    private const int SharedTailValue = 100_000;

    public static (SinglyLinkedListNode<int> HeadA, SinglyLinkedListNode<int> HeadB) Build(int prefixLength)
    {
        var shared = new SinglyLinkedListNode<int>(SharedTailValue);
        var headA = Prepend(prefixLength, shared);
        var headB = Prepend(prefixLength * 2, shared);

        return (headA, headB);
    }

    private static SinglyLinkedListNode<int> Prepend(int count, SinglyLinkedListNode<int> tail)
    {
        var head = tail;

        for (var position = 1; position <= count; position++)
        {
            head = new SinglyLinkedListNode<int>(position) { Next = head };
        }

        return head;
    }
}
