using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.SinglyLinkedList.Fixtures;

internal static class SinglyLinkedLists
{
    // 1 -> 2 -> 3 -> 4 -> 5 -> null
    public static SinglyLinkedListNode<int> Acyclic()
    {
        var five = new SinglyLinkedListNode<int>(5);
        var four = new SinglyLinkedListNode<int>(4) { Next = five };
        var three = new SinglyLinkedListNode<int>(3) { Next = four };
        var two = new SinglyLinkedListNode<int>(2) { Next = three };
        return new SinglyLinkedListNode<int>(1) { Next = two };
    }

    // 1 -> 2 -> 3 -> 4 -> back to 2 (the tail rejoins mid-list, not the head).
    public static (SinglyLinkedListNode<int> Head, SinglyLinkedListNode<int> CycleStart) WithCycle()
    {
        var four = new SinglyLinkedListNode<int>(4);
        var three = new SinglyLinkedListNode<int>(3) { Next = four };
        var two = new SinglyLinkedListNode<int>(2) { Next = three };
        four.Next = two;
        var head = new SinglyLinkedListNode<int>(1) { Next = two };
        return (head, two);
    }

    public static SinglyLinkedListNode<int> SingleNodeSelfCycle()
    {
        var node = new SinglyLinkedListNode<int>(1);
        node.Next = node;
        return node;
    }
}
