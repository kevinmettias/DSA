namespace DSAExperimentation.DataStructures.DoublyLinkedList;

// A genuinely mutable reference type, not a record struct like HashMapEntry - intrusive
// splicing needs a stable identity that a composing cache holds a direct reference into
// and mutates in place (Prev/Next reassignment), the same reasoning that argues for a
// reference type whenever the sharing itself is the point. Auto-properties, not bare
// fields, matching this repo's one other mutable-reference-node precedent, TrieNode.
internal sealed class DoublyLinkedListNode<TValue>
{
    public TValue Value { get; set; } = default!;

    public DoublyLinkedListNode<TValue>? Previous { get; set; }

    public DoublyLinkedListNode<TValue>? Next { get; set; }
}
