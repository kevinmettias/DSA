namespace DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

// Mirrors LeetCode's own multilevel doubly linked list Node: Previous/Next walk the
// current level, Child optionally begins an independent, deeper-nested sublist. This
// shape (a doubly linked list node with a THIRD pointer opening an independently
// linked sublist) answers LC 430 alone - DataStructures/DoublyLinkedList's own
// DoublyLinkedListNode<T> has no Child, and generalizing it to one would be
// inventing a primitive no other problem here needs - so it stays in this problem's
// own folder rather than DataStructures/.
internal sealed class Node(int val)
{
    public int Val { get; } = val;

    public Node? Previous { get; set; }

    public Node? Next { get; set; }

    public Node? Child { get; set; }
}
