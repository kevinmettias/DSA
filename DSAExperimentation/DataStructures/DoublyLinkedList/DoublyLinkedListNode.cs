namespace DSAExperimentation.DataStructures.DoublyLinkedList;

// A genuinely mutable reference type, not a record struct like HashMapEntry - intrusive
// splicing needs a stable identity that a composing cache holds a direct reference into
// and mutates in place (Prev/Next reassignment), the same reasoning that argues for a
// reference type whenever the sharing itself is the point. Auto-properties, not bare
// fields, matching this repo's one other mutable-reference-node precedent, TrieNode.
/// <summary>
/// A node carrying a value and links to its neighbors, which a list or a composing structure splices in place.
/// </summary>
public sealed class DoublyLinkedListNode<TValue>
{
    /// <summary>The value this node carries.</summary>
    public TValue Value { get; set; } = default!;

    /// <summary>The node before this one, or null when nothing has been linked before it.</summary>
    public DoublyLinkedListNode<TValue>? Previous { get; set; }

    /// <summary>The node after this one, or null when nothing has been linked after it.</summary>
    public DoublyLinkedListNode<TValue>? Next { get; set; }
}
