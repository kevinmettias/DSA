using DSAExperimentation.DataStructures.LinkedList;

namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// The one linked node with a forward link and a random one, the shape LC 138 (Copy List with Random
// Pointer) needs: every node carries one extra reference that can point anywhere in the list -
// including itself, or nowhere at all - alongside the ordinary Next chain. Random is the field this
// configuration adds to the one linked node (ARCHITECTURE.md §13.11), settable in place because it
// is assigned after every node in the list already exists.
/// <summary>
/// A linked list node with an extra <c>Random</c> link, which may point at any node of the list or at none.
/// </summary>
/// <param name="value">The value the node starts with.</param>
public sealed class RandomLinkedListNode<TValue>(TValue value)
    : LinkedNode<RandomLinkedListNode<TValue>, TValue>(value)
{
    /// <summary>The extra link: any node of the list, this one included, or null.</summary>
    public RandomLinkedListNode<TValue>? Random { get; set; }
}
