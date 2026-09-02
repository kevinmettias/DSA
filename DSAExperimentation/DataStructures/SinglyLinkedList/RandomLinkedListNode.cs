namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// Sibling of SinglyLinkedListNode<TValue> for the shape LC 138 (Copy List with
// Random Pointer) needs: every node carries one extra reference that can point
// anywhere in the list - including itself, or nowhere at all - alongside the
// ordinary Next chain. A genuinely mutable reference type for the same reason
// DoublyLinkedListNode's Previous is: the extra pointer is assigned after every
// node in the list already exists, so it has to be settable in place rather than
// fixed at construction.
internal sealed class RandomLinkedListNode<TValue>(TValue value)
{
    public TValue Value { get; set; } = value;

    public RandomLinkedListNode<TValue>? Next { get; set; }

    public RandomLinkedListNode<TValue>? Random { get; set; }
}
