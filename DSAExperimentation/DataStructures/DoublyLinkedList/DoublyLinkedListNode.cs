using DSAExperimentation.DataStructures.LinkedList;

namespace DSAExperimentation.DataStructures.DoublyLinkedList;

// The one linked node with a back link beside its forward link (ARCHITECTURE.md §13.11): Previous
// is the field this configuration adds, and the singly linked node, the same template without it,
// has no back link to keep or write. A genuinely mutable reference type, not a record struct like
// HashMapEntry - intrusive splicing needs a stable identity that a composing cache holds a direct
// reference into and mutates in place (Previous/Next reassignment), the same reasoning that argues
// for a reference type whenever the sharing itself is the point. Built with an object initializer,
// { Value = ... }, so the value starts as its default.
/// <summary>
/// A node carrying a value and links to its neighbors, which a list or a composing structure splices in place.
/// </summary>
public sealed class DoublyLinkedListNode<TValue> : LinkedNode<DoublyLinkedListNode<TValue>, TValue>
{
    /// <summary>The node before this one, or null when nothing has been linked before it.</summary>
    public DoublyLinkedListNode<TValue>? Previous { get; set; }
}
