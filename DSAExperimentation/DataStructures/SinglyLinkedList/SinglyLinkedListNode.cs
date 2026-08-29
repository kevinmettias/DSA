namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// Mirrors DoublyLinkedListNode minus Previous: a genuinely mutable reference type
// (reference identity is what CycleDetection.FindCycleStart's ReferenceEquals
// walk needs), settable Value - many list problems rewrite a node's value in
// place (e.g. Delete Node in a Linked List copies the next node's value into the
// current node), not just its Next. Primary constructor for the common "new node
// with this value" call shape, matching BinaryTreeNode's precedent.
internal sealed class SinglyLinkedListNode<TValue>(TValue value)
{
    public TValue Value { get; set; } = value;

    public SinglyLinkedListNode<TValue>? Next { get; set; }
}
