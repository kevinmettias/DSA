namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// Sibling of SinglyLinkedListNode<TValue> for the shape LC 138 (Copy List with
// Random Pointer) needs: every node carries one extra reference that can point
// anywhere in the list - including itself, or nowhere at all - alongside the
// ordinary Next chain. A genuinely mutable reference type for the same reason
// DoublyLinkedListNode's Previous is: the extra pointer is assigned after every
// node in the list already exists, so it has to be settable in place rather than
// fixed at construction.
/// <summary>
/// A linked list node with an extra <c>Random</c> link, which may point at any node of the list or at none.
/// </summary>
/// <param name="value">The value the node starts with.</param>
public sealed class RandomLinkedListNode<TValue>(TValue value)
{
    /// <summary>The value the node holds.</summary>
    public TValue Value { get; set; } = value;

    /// <summary>The next node in the list, or null when this node is the last.</summary>
    public RandomLinkedListNode<TValue>? Next { get; set; }

    /// <summary>The extra link: any node of the list, this one included, or null.</summary>
    public RandomLinkedListNode<TValue>? Random { get; set; }
}
